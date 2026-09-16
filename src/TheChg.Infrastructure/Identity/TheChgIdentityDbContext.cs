using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheChg.Domain.Organization;

namespace TheChg.Infrastructure.Identity;

public sealed class TheChgIdentityDbContext(DbContextOptions<TheChgIdentityDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<AccountInvitation> Invitations => Set<AccountInvitation>();

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

        modelBuilder.Entity<AccountInvitation>(entity =>
        {
            entity.ToTable("AccountInvitations", "identity", table =>
            {
                table.HasCheckConstraint("CK_AccountInvitation_Expiry", "[ExpiresAt] > '2000-01-01'");
            });
            entity.HasKey(invitation => invitation.Id);
            entity.Property(invitation => invitation.Id).ValueGeneratedNever();
            entity.Property(invitation => invitation.NormalizedDestination).HasMaxLength(256).IsRequired();
            entity.Property(invitation => invitation.TokenHash).HasMaxLength(64).IsRequired();
            entity.HasIndex(invitation => invitation.TokenHash).IsUnique();
            entity.HasIndex(invitation => new { invitation.UserId, invitation.ConsumedAt });
            entity.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(invitation => new { invitation.ChurchId, invitation.UserId })
                .HasPrincipalKey(user => new { user.ChurchId, user.Id })
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Church>().WithMany().HasForeignKey(invitation => invitation.ChurchId)
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
