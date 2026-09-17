using System.ComponentModel.DataAnnotations;
using TheChg.Application.Authorization;
using TheChg.Application.Organization;
using TheChg.Domain.Organization;

namespace TheChg.Application.Registration;

public enum BranchAccountKind
{
    Member = 1,
    Staff = 2
}

public sealed record PendingAccountRegistration(Guid Id, Guid AccountId, Guid BranchId, BranchAccountKind Kind);

public interface IAccountRegistrationWriter
{
    Task<PendingAccountRegistration> CreatePendingAsync(Guid registrarId, Guid churchId,
        Guid branchId, BranchAccountKind kind, string email, CancellationToken cancellationToken);
}

public sealed class DuplicateAccountRegistrationException : Exception
{
    public DuplicateAccountRegistrationException() : base("An account already exists for this email address.") { }
}

/// <summary>Captures a request at an authorized South African branch; never activates an account or grants access.</summary>
public sealed class BranchAccountRegistrationService(
    IOrganizationRepository organization,
    ScopedAuthorizationEvaluator authorization,
    IAccountRegistrationWriter writer)
{
    public async Task<PendingAccountRegistration?> CaptureAsync(Guid registrarId, Guid branchId,
        BranchAccountKind kind, string email, CancellationToken cancellationToken = default)
    {
        if (registrarId == Guid.Empty || branchId == Guid.Empty ||
            kind is not (BranchAccountKind.Member or BranchAccountKind.Staff) ||
            string.IsNullOrWhiteSpace(email) || email.Trim().Length > 256 ||
            !new EmailAddressAttribute().IsValid(email.Trim()))
            throw new ArgumentException("A registrar, branch, account kind and valid email are required.");

        var branch = await organization.FindUnitAsync(branchId, cancellationToken);
        if (branch is null || branch.UnitType != OrganizationUnitType.Branch || branch.CountryCode != "ZA")
            return null;

        var permission = kind == BranchAccountKind.Member ? "Member.Create" : "Staff.Register";
        if (!await authorization.IsAllowedAsync(registrarId, permission,
                new AuthorizationResource(branch.ChurchId, branch.Id), cancellationToken))
            return null;

        return await writer.CreatePendingAsync(registrarId, branch.ChurchId, branch.Id, kind,
            email.Trim(), cancellationToken);
    }
}
