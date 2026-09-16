using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TheChg.Web.Tests;

public sealed class PublicEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Theory]
    [InlineData("/")]
    [InlineData("/health/live")]
    [InlineData("/Account/Login")]
    public async Task Public_endpoints_are_available_without_a_database_query(string path)
    {
        var response = await factory.CreateClient().GetAsync(path);
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
    public async Task Login_post_without_antiforgery_token_is_rejected()
    {
        var response = await factory.CreateClient().PostAsync("/Account/Login",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Email"] = "synthetic@example.test",
                ["Password"] = "NotARealPassword1!"
            }));

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}
