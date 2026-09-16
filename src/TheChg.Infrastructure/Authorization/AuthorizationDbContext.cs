using Microsoft.EntityFrameworkCore;
using TheChg.Domain.Organization;
using TheChg.Infrastructure.Identity;

namespace TheChg.Infrastructure.Authorization;

public sealed class AuthorizationDbContext(DbContextOptions<AuthorizationDbContext> options) : DbContext(options)
{
    public DbSet<PermissionGrantRecord> PermissionGrants => Set<PermissionGrantRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // These principals are owned and migrated by their own DbContexts. Keeping them
        // in this model makes the cross-schema FKs explicit and prevents migration drift.
        modelBuilder.Entity<Church>(entity =>
        {
            entity.ToTable("Churches", "organization", table => table.ExcludeFromMigrations());
            entity.HasKey(church => church.Id);
        });
        modelBuilder.Entity<OrganizationUnit>(entity =>
        {
            entity.ToTable("OrganizationalUnits", "organization", table => table.ExcludeFromMigrations());
            entity.HasKey(unit => unit.Id);
            entity.HasAlternateKey(unit => new { unit.ChurchId, unit.Id });
        });
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users", "identity", table => table.ExcludeFromMigrations());
            entity.HasKey(user => user.Id);
            entity.HasAlternateKey(user => new { user.ChurchId, user.Id });
        });

        modelBuilder.Entity<PermissionGrantRecord>(entity =>
        {
            entity.ToTable("PermissionGrants", "authorization", table =>
            {
                table.HasCheckConstraint("CK_PermissionGrant_NonemptyIds",
                    "[Id] <> '00000000-0000-0000-0000-000000000000' AND " +
                    "[AccountId] <> '00000000-0000-0000-0000-000000000000' AND " +
                    "[ChurchId] <> '00000000-0000-0000-0000-000000000000' AND " +
                    "([ScopeUnitId] IS NULL OR [ScopeUnitId] <> '00000000-0000-0000-0000-000000000000')");
                table.HasCheckConstraint("CK_PermissionGrant_EffectiveRange",
                    "[EffectiveUntil] IS NULL OR [EffectiveUntil] > [EffectiveFrom]");
                table.HasCheckConstraint("CK_PermissionGrant_ScopeShape",
                    "[ScopeUnitId] IS NOT NULL OR [IncludeDescendants] = 0");
                table.HasCheckConstraint("CK_PermissionGrant_Permission",
                    "TRIM([Permission]) <> '' AND [Permission] = TRIM([Permission])");
            });
            entity.HasKey(grant => grant.Id);
            entity.Property(grant => grant.Id).ValueGeneratedNever();
            entity.Property(grant => grant.Permission).HasMaxLength(128).IsRequired();
            entity.Property(grant => grant.EffectiveFrom).IsRequired();
            entity.HasIndex(grant => new { grant.AccountId, grant.ChurchId, grant.Permission });
            entity.HasIndex(grant => new { grant.AccountId, grant.ChurchId, grant.Permission,
                grant.ScopeUnitId, grant.EffectiveFrom }).IsUnique().HasFilter(null);
            entity.HasIndex(grant => new { grant.ChurchId, grant.ScopeUnitId });
            entity.HasOne<Church>().WithMany().HasForeignKey(grant => grant.ChurchId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(grant => new { grant.ChurchId, grant.AccountId })
                .HasPrincipalKey(user => new { user.ChurchId, user.Id })
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<OrganizationUnit>().WithMany()
                .HasForeignKey(grant => new { grant.ChurchId, grant.ScopeUnitId })
                .HasPrincipalKey(unit => new { unit.ChurchId, unit.Id })
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
