using Microsoft.EntityFrameworkCore;
using TheChg.Domain.Organization;

namespace TheChg.Infrastructure.Organization;

public sealed class OrganizationDbContext(DbContextOptions<OrganizationDbContext> options) : DbContext(options)
{
    public DbSet<Church> Churches => Set<Church>();
    public DbSet<OrganizationUnit> Units => Set<OrganizationUnit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Church>(entity =>
        {
            entity.ToTable("Churches", "organization");
            entity.HasKey(church => church.Id);
            entity.Property(church => church.Id).ValueGeneratedNever();
            entity.Property(church => church.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<OrganizationUnit>(entity =>
        {
            entity.ToTable("OrganizationalUnits", "organization", table =>
                table.HasCheckConstraint("CK_OrganizationalUnit_ParentShape",
                    "([UnitType] = 1 AND [ParentId] IS NULL) OR ([UnitType] IN (2, 3) AND [ParentId] IS NOT NULL)"));
            entity.HasKey(unit => unit.Id);
            entity.Property(unit => unit.Id).ValueGeneratedNever();
            entity.Property(unit => unit.UnitType).HasConversion<int>().IsRequired();
            entity.Property(unit => unit.Code).HasMaxLength(32).IsRequired();
            entity.Property(unit => unit.Name).HasMaxLength(200).IsRequired();
            entity.Property(unit => unit.CountryCode).HasMaxLength(2).IsRequired();
            entity.Property(unit => unit.TimeZoneId).HasMaxLength(100).IsRequired();
            entity.Property(unit => unit.CurrencyCode).HasMaxLength(3).IsRequired();

            entity.HasOne<Church>().WithMany().HasForeignKey(unit => unit.ChurchId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasAlternateKey(unit => new { unit.ChurchId, unit.Id });
            entity.HasOne<OrganizationUnit>().WithMany()
                .HasForeignKey(unit => new { unit.ChurchId, unit.ParentId })
                .HasPrincipalKey(unit => new { unit.ChurchId, unit.Id })
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(unit => new { unit.ChurchId, unit.ParentId, unit.Code })
                .IsUnique().HasFilter(null);
            entity.HasIndex(unit => new { unit.ChurchId, unit.Code })
                .IsUnique().HasFilter("[UnitType] = 1");
        });
    }
}
