using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TheChg.Infrastructure.Identity;

public sealed class TheChgIdentityDbContext(DbContextOptions<TheChgIdentityDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users", "identity", table =>
                table.HasCheckConstraint("CK_IdentityUser_ChurchId", "[ChurchId] <> '00000000-0000-0000-0000-000000000000'"));
            entity.Property(user => user.ChurchId).IsRequired();
            entity.Property(user => user.IsActive).HasDefaultValue(false).IsRequired();
            entity.HasIndex(user => user.ChurchId);
            entity.HasIndex(user => user.NormalizedEmail)
                .IsUnique().HasFilter("[NormalizedEmail] IS NOT NULL");
        });

        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles", "identity");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles", "identity");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims", "identity");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins", "identity");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims", "identity");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens", "identity");
    }
}
