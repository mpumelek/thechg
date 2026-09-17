using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheChg.Domain.Organization;

namespace TheChg.Infrastructure.Identity;

public sealed class TheChgIdentityDbContext(DbContextOptions<TheChgIdentityDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<BranchAccountRegistration> BranchAccountRegistrations => Set<BranchAccountRegistration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Organization owns the table and its migrations; Identity only references its key.
        modelBuilder.Entity<Church>(entity =>
        {
            entity.ToTable("Churches", "organization", table => table.ExcludeFromMigrations());
            entity.HasKey(church => church.Id);
            entity.Property(church => church.Id).ValueGeneratedNever();
            entity.Property(church => church.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<OrganizationUnit>(entity =>
        {
            entity.ToTable("OrganizationalUnits", "organization", table => table.ExcludeFromMigrations());
            entity.HasKey(unit => unit.Id);
            entity.HasAlternateKey(unit => new { unit.ChurchId, unit.Id });
        });

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users", "identity", table =>
                table.HasCheckConstraint("CK_IdentityUser_ChurchId", "[ChurchId] <> '00000000-0000-0000-0000-000000000000'"));
            entity.Property(user => user.ChurchId).IsRequired();
            entity.Property(user => user.IsActive).HasDefaultValue(false).IsRequired();
            entity.HasIndex(user => user.ChurchId);
            entity.HasAlternateKey(user => new { user.ChurchId, user.Id });
            entity.HasOne<Church>().WithMany().HasForeignKey(user => user.ChurchId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(user => user.NormalizedEmail)
                .IsUnique().HasFilter("[NormalizedEmail] IS NOT NULL");
        });

        modelBuilder.Entity<BranchAccountRegistration>(entity =>
        {
            entity.ToTable("BranchAccountRegistrations", "identity", table =>
                table.HasCheckConstraint("CK_BranchAccountRegistration_Kind", "[Kind] IN (1, 2)"));
            entity.HasKey(registration => registration.Id);
            entity.Property(registration => registration.Id).ValueGeneratedNever();
            entity.Property(registration => registration.Kind).HasConversion<int>().IsRequired();
            entity.Property(registration => registration.CapturedAt).IsRequired();
            entity.HasIndex(registration => registration.AccountId).IsUnique();
            entity.HasIndex(registration => new { registration.ChurchId, registration.BranchId });
            entity.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(registration => new { registration.ChurchId, registration.AccountId })
                .HasPrincipalKey(user => new { user.ChurchId, user.Id })
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(registration => new { registration.ChurchId, registration.RegistrarId })
                .HasPrincipalKey(user => new { user.ChurchId, user.Id })
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Church>().WithMany().HasForeignKey(registration => registration.ChurchId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<OrganizationUnit>().WithMany()
                .HasForeignKey(registration => new { registration.ChurchId, registration.BranchId })
                .HasPrincipalKey(unit => new { unit.ChurchId, unit.Id })
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles", "identity");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles", "identity");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims", "identity");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins", "identity");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims", "identity");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens", "identity");
    }
}
