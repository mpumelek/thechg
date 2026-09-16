namespace TheChg.Domain.Organization;

public sealed class OrganizationUnit
{
    private OrganizationUnit() { } // EF Core

    private OrganizationUnit(Guid id, Guid churchId, Guid? parentId, OrganizationUnitType unitType,
        string code, string name, string countryCode, string timeZoneId, string currencyCode)
    {
        Id = id;
        ChurchId = churchId;
        ParentId = parentId;
        UnitType = unitType;
        Code = code;
        Name = name;
        CountryCode = countryCode;
        TimeZoneId = timeZoneId;
        CurrencyCode = currencyCode;
    }

    public Guid Id { get; private set; }
    public Guid ChurchId { get; private set; }
    public Guid? ParentId { get; private set; }
    public OrganizationUnitType UnitType { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string CountryCode { get; private set; } = null!;
    public string TimeZoneId { get; private set; } = null!;
    public string CurrencyCode { get; private set; } = null!;

    public static OrganizationUnit CreateCountry(Guid id, Church church, string countryCode,
        string name, string timeZoneId, string currencyCode)
    {
        ArgumentNullException.ThrowIfNull(church);
        var normalizedCountryCode = NormalizeLetters(countryCode, 2, nameof(countryCode));
        var normalizedCurrencyCode = NormalizeLetters(currencyCode, 3, nameof(currencyCode));
        return new OrganizationUnit(RequireId(id), church.Id, null, OrganizationUnitType.Country,
            normalizedCountryCode, RequireName(name), normalizedCountryCode,
            RequireTimeZone(timeZoneId), normalizedCurrencyCode);
    }

    public static OrganizationUnit CreateCircuit(Guid id, OrganizationUnit country, string code, string name) =>
        CreateChild(id, country, OrganizationUnitType.Country, OrganizationUnitType.Circuit, code, name);

    public static OrganizationUnit CreateBranch(Guid id, OrganizationUnit circuit, string code, string name) =>
        CreateChild(id, circuit, OrganizationUnitType.Circuit, OrganizationUnitType.Branch, code, name);

    private static OrganizationUnit CreateChild(Guid id, OrganizationUnit parent, OrganizationUnitType expectedParent,
        OrganizationUnitType unitType, string code, string name)
    {
        ArgumentNullException.ThrowIfNull(parent);
        if (parent.UnitType != expectedParent)
            throw new ArgumentException($"A {unitType} must be placed under a {expectedParent}.", nameof(parent));

        return new OrganizationUnit(RequireId(id), parent.ChurchId, parent.Id, unitType,
            NormalizeCode(code), RequireName(name), parent.CountryCode, parent.TimeZoneId, parent.CurrencyCode);
    }

    private static Guid RequireId(Guid id) =>
        id == Guid.Empty ? throw new ArgumentException("An organization unit ID is required.", nameof(id)) : id;

    private static string RequireName(string name) =>
        string.IsNullOrWhiteSpace(name) || name.Trim().Length > 200
            ? throw new ArgumentException("A name of at most 200 characters is required.", nameof(name))
            : name.Trim();

    private static string RequireTimeZone(string timeZoneId) =>
        string.IsNullOrWhiteSpace(timeZoneId) || timeZoneId.Trim().Length > 100
            ? throw new ArgumentException("A time zone ID of at most 100 characters is required.", nameof(timeZoneId))
            : timeZoneId.Trim();

    private static string NormalizeLetters(string value, int length, string parameterName)
    {
        var normalized = value?.Trim().ToUpperInvariant();
        if (normalized is null || normalized.Length != length || normalized.Any(character => character is not (>= 'A' and <= 'Z')))
            throw new ArgumentException($"A {length}-letter ISO code is required.", parameterName);
        return normalized;
    }

    private static string NormalizeCode(string code)
    {
        var normalized = code?.Trim().ToUpperInvariant();
        if (string.IsNullOrEmpty(normalized) || normalized.Length > 32 ||
            normalized.Any(character => character is not (>= 'A' and <= 'Z') and not (>= '0' and <= '9') and not '-'))
            throw new ArgumentException("A code of 1–32 letters, digits or hyphens is required.", nameof(code));
        return normalized;
    }
}
