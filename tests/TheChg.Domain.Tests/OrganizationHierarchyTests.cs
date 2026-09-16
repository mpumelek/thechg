using TheChg.Domain.Organization;

namespace TheChg.Domain.Tests;

public sealed class OrganizationHierarchyTests
{
    private static readonly Church Church = Church.Create(Guid.NewGuid(), "Test denomination");

    [Fact]
    public void Branch_inherits_country_configuration_and_church_scope()
    {
        var country = OrganizationUnit.CreateCountry(Guid.NewGuid(), Church, "za", "South Africa",
            "Africa/Johannesburg", "zar");
        var circuit = OrganizationUnit.CreateCircuit(Guid.NewGuid(), country, "c-01", "Circuit 1");
        var branch = OrganizationUnit.CreateBranch(Guid.NewGuid(), circuit, "b-01", "Branch 1");

        Assert.Null(country.ParentId);
        Assert.Equal(country.Id, circuit.ParentId);
        Assert.Equal(circuit.Id, branch.ParentId);
        Assert.Equal(Church.Id, branch.ChurchId);
        Assert.Equal("ZA", branch.CountryCode);
        Assert.Equal("ZAR", branch.CurrencyCode);
        Assert.Equal("Africa/Johannesburg", branch.TimeZoneId);
        Assert.Equal("B-01", branch.Code);
    }

    [Fact]
    public void Invalid_parent_types_are_rejected()
    {
        var country = OrganizationUnit.CreateCountry(Guid.NewGuid(), Church, "ZA", "South Africa",
            "Africa/Johannesburg", "ZAR");
        var circuit = OrganizationUnit.CreateCircuit(Guid.NewGuid(), country, "C-01", "Circuit 1");

        Assert.Throws<ArgumentException>(() => OrganizationUnit.CreateBranch(Guid.NewGuid(), country, "B-01", "Branch"));
        Assert.Throws<ArgumentException>(() => OrganizationUnit.CreateCircuit(Guid.NewGuid(), circuit, "C-02", "Circuit"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("bad code")]
    public void Invalid_codes_are_rejected(string code)
    {
        var country = OrganizationUnit.CreateCountry(Guid.NewGuid(), Church, "ZA", "South Africa",
            "Africa/Johannesburg", "ZAR");
        var circuit = OrganizationUnit.CreateCircuit(Guid.NewGuid(), country, "C-01", "Circuit");
        Assert.Throws<ArgumentException>(() => OrganizationUnit.CreateBranch(Guid.NewGuid(), circuit, code, "Branch"));
    }

    [Fact]
    public void Empty_ids_and_invalid_iso_codes_are_rejected()
    {
        Assert.Throws<ArgumentException>(() => Church.Create(Guid.Empty, "Denomination"));
        Assert.Throws<ArgumentException>(() => OrganizationUnit.CreateCountry(Guid.NewGuid(), Church, "ZAF",
            "South Africa", "Africa/Johannesburg", "ZAR"));
        Assert.Throws<ArgumentException>(() => OrganizationUnit.CreateCountry(Guid.NewGuid(), Church, "ZA",
            "South Africa", "Africa/Johannesburg", "R"));
    }
}
