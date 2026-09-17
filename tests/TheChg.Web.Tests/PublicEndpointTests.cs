using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace TheChg.Web.Tests;

public sealed class PublicEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Theory]
    [InlineData("/")]
    [InlineData("/health/live")]
    [InlineData("/Account/Login")]
    public async Task Public_endpoints_are_available_without_a_database_query(string path)
    {
        var response = await SecureClient().GetAsync(path);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public void Unsafe_browser_methods_get_global_antiforgery_validation()
    {
        var options = factory.Services.GetRequiredService<IOptions<MvcOptions>>().Value;
        Assert.Contains(options.Filters, filter => filter is AutoValidateAntiforgeryTokenAttribute);
    }

    [Fact]
    public void Unmarked_endpoints_require_authentication()
    {
        var options = factory.Services.GetRequiredService<IOptions<AuthorizationOptions>>().Value;
        Assert.NotNull(options.FallbackPolicy);
        Assert.Contains(options.FallbackPolicy.Requirements,
            requirement => requirement is DenyAnonymousAuthorizationRequirement);
    }

    [Fact]
    public void Authentication_cookie_is_http_only_and_https_only()
    {
        var options = factory.Services.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
            .Get(IdentityConstants.ApplicationScheme);
        Assert.True(options.Cookie.HttpOnly);
        Assert.Equal(CookieSecurePolicy.Always, options.Cookie.SecurePolicy);
        Assert.False(options.SlidingExpiration);
    }

    [Fact]
    public void Antiforgery_cookie_and_header_use_explicit_browser_controls()
    {
        var options = factory.Services.GetRequiredService<IOptions<AntiforgeryOptions>>().Value;
        Assert.Equal("X-CSRF-TOKEN", options.HeaderName);
        Assert.Equal(CookieSecurePolicy.Always, options.Cookie.SecurePolicy);
        Assert.Equal(SameSiteMode.Strict, options.Cookie.SameSite);
        Assert.True(options.Cookie.HttpOnly);
    }

    [Fact]
    public async Task Login_post_without_antiforgery_token_is_rejected()
    {
        var response = await SecureClient().PostAsync("/Account/Login",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Email"] = "synthetic@example.test",
                ["Password"] = "NotARealPassword1!"
            }));

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Anonymous_branch_account_registration_is_denied()
    {
        var response = await SecureClient().PostAsJsonAsync("/api/v1/registrations/accounts",
            new { branchId = Guid.NewGuid(), kind = "Member", email = "synthetic@example.test" });

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Anonymous_antiforgery_token_request_is_denied()
    {
        var response = await SecureClient().GetAsync("/api/v1/security/antiforgery");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private HttpClient SecureClient() => factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost")
    });
}
