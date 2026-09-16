namespace TheChg.Contracts.Organization;

public sealed record BranchDetailsResponse(
    Guid Id,
    string Code,
    string Name,
    Guid CircuitId,
    string CountryCode);
