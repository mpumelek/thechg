using TheChg.Application.Registration;

namespace TheChg.Infrastructure.Identity;

/// <summary>Audit evidence of an in-person branch account request, not Church membership or staff access.</summary>
public sealed class BranchAccountRegistration
{
    private BranchAccountRegistration() { } // EF Core

    internal BranchAccountRegistration(Guid registrarId, Guid accountId, Guid churchId,
        Guid branchId, BranchAccountKind kind, DateTime capturedAt)
    {
        Id = Guid.NewGuid();
        RegistrarId = registrarId;
        AccountId = accountId;
        ChurchId = churchId;
        BranchId = branchId;
        Kind = kind;
        CapturedAt = capturedAt;
    }

    public Guid Id { get; private set; }
    public Guid RegistrarId { get; private set; }
    public Guid AccountId { get; private set; }
    public Guid ChurchId { get; private set; }
    public Guid BranchId { get; private set; }
    public BranchAccountKind Kind { get; private set; }
    public DateTime CapturedAt { get; private set; }
}
