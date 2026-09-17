using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheChg.Application.Registration;
using TheChg.Contracts.Registration;
using TheChg.Infrastructure.Identity;

namespace TheChg.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/registrations/accounts")]
public sealed class AccountRegistrationsController(
    BranchAccountRegistrationService registrations,
    UserManager<ApplicationUser> users) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Capture(
        CaptureAccountRegistrationRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(users.GetUserId(User), out var registrarId))
            return Unauthorized();

        if (!Enum.TryParse<BranchAccountKind>(request.Kind, ignoreCase: false, out var kind) ||
            kind is not (BranchAccountKind.Member or BranchAccountKind.Staff))
            return BadRequest();

        try
        {
            var registration = await registrations.CaptureAsync(registrarId, request.BranchId,
                kind, request.Email, cancellationToken);
            if (registration is null)
                return Forbid();

            return StatusCode(StatusCodes.Status201Created,
                new PendingAccountRegistrationResponse(registration.Id, registration.AccountId,
                    registration.BranchId, registration.Kind.ToString(), "PendingReview"));
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (DuplicateAccountRegistrationException)
        {
            return Conflict();
        }
    }
}
