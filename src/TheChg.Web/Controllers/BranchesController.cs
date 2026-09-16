using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheChg.Application.Organization;
using TheChg.Infrastructure.Identity;

namespace TheChg.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/organization/branches")]
public sealed class BranchesController(
    BranchDetailsService branches,
    UserManager<ApplicationUser> users) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(users.GetUserId(User), out var accountId))
            return Unauthorized();

        var branch = await branches.GetAsync(accountId, id, cancellationToken);
        return branch is null ? Forbid() : Ok(branch);
    }
}
