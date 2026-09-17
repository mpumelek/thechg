using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TheChg.Web.Controllers;

/// <summary>Issues an authenticated request token for browser JSON writes.</summary>
[ApiController]
[Authorize]
[Route("api/v1/security/antiforgery")]
public sealed class AntiforgeryController(IAntiforgery antiforgery) : ControllerBase
{
    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult Get()
    {
        var token = antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
        return Ok(new { requestToken = token });
    }
}
