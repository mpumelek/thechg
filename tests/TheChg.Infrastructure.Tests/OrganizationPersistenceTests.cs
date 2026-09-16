using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TheChg.Domain.Organization;
using TheChg.Infrastructure.Organization;

namespace TheChg.Infrastructure.Tests;

public sealed class OrganizationPersistenceTests
{
    [Fact]
    public async Task Hierarchy_round_trips_through_an_isolated_database()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var db = CreateContext(connection);
        await db.Database.EnsureCreatedAsync();
        var church = Church.Create(Guid.NewGuid(), "Test denomination");
        var country = OrganizationUnit.CreateCountry(Guid.NewGuid(), church, "ZA", "South Africa",
            "Africa/Johannesburg", "ZAR");
        var circuit = OrganizationUnit.CreateCircuit(Guid.NewGuid(), country, "C-01", "Circuit 1");
        var branch = OrganizationUnit.CreateBranch(Guid.NewGuid(), circuit, "B-01", "Branch 1");

        db.Churches.Add(church);
        db.Units.AddRange(country, circuit, branch);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var loaded = await db.Units.AsNoTracking().SingleAsync(unit => unit.Id == branch.Id);
        Assert.Equal(circuit.Id, loaded.ParentId);
        Assert.Equal(church.Id, loaded.ChurchId);
        Assert.Equal("ZA", loaded.CountryCode);
    }

    [Fact]
    public async Task Duplicate_country_code_is_rejected_by_the_database()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var db = CreateContext(connection);
        await db.Database.EnsureCreatedAsync();
        var church = Church.Create(Guid.NewGuid(), "Test denomination");
        db.Churches.Add(church);
        db.Units.Add(OrganizationUnit.CreateCountry(Guid.NewGuid(), church, "ZA", "South Africa",
            "Africa/Johannesburg", "ZAR"));
        await db.SaveChangesAsync();

        db.Units.Add(OrganizationUnit.CreateCountry(Guid.NewGuid(), church, "ZA", "Duplicate",
            "Africa/Johannesburg", "ZAR"));
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task Parent_must_belong_to_the_same_church()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var db = CreateContext(connection);
        await db.Database.EnsureCreatedAsync();
        var firstChurch = Church.Create(Guid.NewGuid(), "First denomination");
        var secondChurch = Church.Create(Guid.NewGuid(), "Second denomination");
        var country = OrganizationUnit.CreateCountry(Guid.NewGuid(), firstChurch, "ZA", "South Africa",
            "Africa/Johannesburg", "ZAR");
        db.Churches.AddRange(firstChurch, secondChurch);
        db.Units.Add(country);
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<SqliteException>(() => db.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO OrganizationalUnits (Id, ChurchId, ParentId, UnitType, Code, Name, CountryCode, TimeZoneId, CurrencyCode) VALUES ({Guid.NewGuid()}, {secondChurch.Id}, {country.Id}, {2}, {"C-01"}, {"Circuit"}, {"ZA"}, {"Africa/Johannesburg"}, {"ZAR"})"));
    }

    private static OrganizationDbContext CreateContext(SqliteConnection connection) =>
        new(new DbContextOptionsBuilder<OrganizationDbContext>().UseSqlite(connection).Options);
}
