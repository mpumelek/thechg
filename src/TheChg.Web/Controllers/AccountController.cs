using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheChg.Infrastructure.Identity;
using TheChg.Web.Models;

namespace TheChg.Web.Controllers;

public sealed class AccountController(
    UserManager<ApplicationUser> users,
    SignInManager<ApplicationUser> signIn) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl : "/");

        return View(new LoginInput { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginInput input)
    {
        if (!ModelState.IsValid)
        {
            ClearPasswordFromResponse(input);
            return View(input);
        }

        var account = await users.FindByEmailAsync(input.Email);
        if (account is not null && account.IsActive && account.EmailConfirmed &&
            account.ChurchId != Guid.Empty && !await users.IsLockedOutAsync(account))
        {
            var result = await signIn.PasswordSignInAsync(account, input.Password,
                isPersistent: false, lockoutOnFailure: true);
            if (result.Succeeded)
                return LocalRedirect(Url.IsLocalUrl(input.ReturnUrl) ? input.ReturnUrl : "/");
        }

        ModelState.AddModelError(string.Empty, "Invalid sign-in details.");
        ClearPasswordFromResponse(input);
        return View(input);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await signIn.SignOutAsync();
        return LocalRedirect("/");
    }

    private void ClearPasswordFromResponse(LoginInput input)
    {
        input.Password = string.Empty;
        ModelState.Remove(nameof(LoginInput.Password));
    }
}
