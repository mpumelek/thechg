using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TheChg.Infrastructure.Identity;
using TheChg.Infrastructure.Organization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
builder.Services.AddHealthChecks();
var connectionString = builder.Configuration.GetConnectionString("ChurchDatabase")
    ?? throw new InvalidOperationException("ConnectionStrings:ChurchDatabase must be configured.");
builder.Services.AddOrganizationPersistence(connectionString);
builder.Services.AddIdentityPersistence(connectionString);
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme, options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = false;
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async context =>
            {
                var identityOptions = context.HttpContext.RequestServices
                    .GetRequiredService<IOptions<IdentityOptions>>().Value;
                var accountId = context.Principal?.FindFirst(identityOptions.ClaimsIdentity.UserIdClaimType)?.Value;
                if (!Guid.TryParse(accountId, out var id))
                {
                    context.RejectPrincipal();
                    return;
                }

                var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                var account = await userManager.FindByIdAsync(id.ToString());
                if (account is null || !account.IsActive || !account.EmailConfirmed ||
                    account.ChurchId == Guid.Empty ||
                    await userManager.IsLockedOutAsync(account))
                {
                    context.RejectPrincipal();
                    return;
                }

                var presentedStamp = context.Principal?
                    .FindFirst(identityOptions.ClaimsIdentity.SecurityStampClaimType)?.Value;
                var currentStamp = await userManager.GetSecurityStampAsync(account);
                if (string.IsNullOrEmpty(presentedStamp) ||
                    !string.Equals(presentedStamp, currentStamp, StringComparison.Ordinal))
                    context.RejectPrincipal();
            },
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization(options =>
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health/live").AllowAnonymous();
app.MapStaticAssets().AllowAnonymous();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

public partial class Program;
