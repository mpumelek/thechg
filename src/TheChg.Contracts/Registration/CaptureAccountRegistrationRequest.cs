namespace TheChg.Contracts.Registration;

public sealed record CaptureAccountRegistrationRequest(Guid BranchId, string Kind, string Email);

public sealed record PendingAccountRegistrationResponse(
    Guid Id, Guid AccountId, Guid BranchId, string Kind, string Status);
