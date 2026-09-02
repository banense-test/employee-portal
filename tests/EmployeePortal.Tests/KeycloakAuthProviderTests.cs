using EmployeePortal.Infrastructure;
using EmployeePortal.Services;
using EmployeePortal.Tests.Fixtures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace EmployeePortal.Tests;

/// <summary>
/// R003 empirical validation against the stub OIDC issuer. Acceptance criteria (PoC decision):
/// the portal completes the OIDC redirect flow; validates a signed token via the issuer's JWKS;
/// extracts the Employee and HR Administrator roles from claims (SEC-006); rejects expired/invalid
/// tokens at the request boundary. Black-box acceptance suite + white-box branch coverage.
/// </summary>
public class KeycloakAuthProviderTests
{
    private const string Authority = "https://stub-issuer.test/realms/employee-portal";
    private const string ClientId = "employee-portal";
    private const string CallbackPath = "/signin-oidc";

    private static KeycloakClientOptions Options() => new(Authority, ClientId, CallbackPath);

    private static (KeycloakAuthProvider Provider, StubOidcIssuer Issuer) Create()
    {
        var issuer = new StubOidcIssuer(Authority, ClientId);
        return (new KeycloakAuthProvider(issuer, issuer, Options()), issuer);
    }

    private static HttpContext ContextWithBearer(string token)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Authorization = "Bearer " + token;
        return context;
    }

    // ---- black-box: the R003 acceptance criteria ----

    [Fact]
    public async Task RedirectFlow_Completes_AgainstStubIssuer()
    {
        var (provider, issuer) = Create();

        // The portal's half of the redirect: the authorize URL construction.
        var authorizeUrl = provider.BuildAuthorizeRedirectUrl(CallbackPath, "state-123");
        Assert.StartsWith(Authority + "/protocol/openid-connect/auth", authorizeUrl);
        Assert.Contains("client_id=employee-portal", authorizeUrl);
        Assert.Contains("response_type=code", authorizeUrl);
        Assert.Contains("state=state-123", authorizeUrl);

        // The issuer's half: the user authenticates; the callback carries a code.
        var code = issuer.IssueAuthorizationCode("u001", [RoleNames.Employee, RoleNames.HrAdministrator],
            DateTimeOffset.UtcNow.AddHours(1));

        // The portal completes the flow: code -> token -> JWKS validation -> identity + roles.
        var user = await provider.HandleOidcCallbackAsync(code);
        Assert.Equal("u001", user.Uid);
        Assert.Contains(RoleNames.Employee, user.Roles);
        Assert.Contains(RoleNames.HrAdministrator, user.Roles);
    }

    [Fact]
    public async Task ValidToken_ValidatedViaIssuerJwks_RolesExtracted()
    {
        var (provider, issuer) = Create();
        var token = issuer.IssueToken("u042", [RoleNames.Employee, RoleNames.HrAdministrator],
            DateTimeOffset.UtcNow.AddHours(1));

        var user = await provider.GetAuthenticatedUserAsync(ContextWithBearer(token));

        Assert.NotNull(user);
        Assert.Equal("u042", user.Uid);
        Assert.Contains(RoleNames.Employee, user.Roles);          // SEC-006
        Assert.Contains(RoleNames.HrAdministrator, user.Roles);   // SEC-006
    }

    [Fact]
    public async Task ExpiredToken_RejectedAtRequestBoundary()
    {
        var (provider, issuer) = Create();
        var token = issuer.IssueToken("u001", [RoleNames.Employee], DateTimeOffset.UtcNow.AddHours(-1));

        await Assert.ThrowsAsync<OidcTokenValidationException>(() => provider.GetAuthenticatedUserAsync(ContextWithBearer(token)));
    }

    [Fact]
    public async Task TamperedToken_RejectedAtRequestBoundary()
    {
        var (provider, issuer) = Create();
        var token = issuer.IssueToken("u001", [RoleNames.Employee], DateTimeOffset.UtcNow.AddHours(1), tamperSignature: true);

        await Assert.ThrowsAsync<OidcTokenValidationException>(() => provider.GetAuthenticatedUserAsync(ContextWithBearer(token)));
    }

    [Fact]
    public async Task WrongIssuerToken_Rejected()
    {
        var (provider, issuer) = Create();
        var token = issuer.IssueToken("u001", [RoleNames.Employee], DateTimeOffset.UtcNow.AddHours(1),
            issuerOverride: "https://evil-issuer.test/realms/other");

        await Assert.ThrowsAsync<OidcTokenValidationException>(() => provider.GetAuthenticatedUserAsync(ContextWithBearer(token)));
    }

    [Fact]
    public async Task WrongAudienceToken_Rejected()
    {
        var (provider, issuer) = Create();
        var token = issuer.IssueToken("u001", [RoleNames.Employee], DateTimeOffset.UtcNow.AddHours(1),
            audienceOverride: "some-other-client");

        await Assert.ThrowsAsync<OidcTokenValidationException>(() => provider.GetAuthenticatedUserAsync(ContextWithBearer(token)));
    }

    [Fact]
    public async Task AlgNoneToken_Rejected()
    {
        var (provider, issuer) = Create();
        var token = issuer.IssueToken("u001", [RoleNames.Employee], DateTimeOffset.UtcNow.AddHours(1),
            algorithmOverride: "none");

        await Assert.ThrowsAsync<OidcTokenValidationException>(() => provider.GetAuthenticatedUserAsync(ContextWithBearer(token)));
    }

    [Fact]
    public async Task UnknownKidToken_Rejected()
    {
        var (provider, issuer) = Create();
        var token = issuer.IssueToken("u001", [RoleNames.Employee], DateTimeOffset.UtcNow.AddHours(1),
            keyIdOverride: "rotated-away-key");

        await Assert.ThrowsAsync<OidcTokenValidationException>(() => provider.GetAuthenticatedUserAsync(ContextWithBearer(token)));
    }

    [Fact]
    public async Task MalformedToken_Rejected()
    {
        var (provider, _) = Create();

        await Assert.ThrowsAsync<OidcTokenValidationException>(() => provider.GetAuthenticatedUserAsync(ContextWithBearer("not-a-jwt")));
    }

    [Fact]
    public async Task MissingBearerToken_ReturnsNull_UnauthenticatedIsNotAnError()
    {
        var (provider, _) = Create();

        var user = await provider.GetAuthenticatedUserAsync(new DefaultHttpContext());

        Assert.Null(user); // unauthenticated -> the middleware challenges; only invalid tokens are errors
    }

    [Fact]
    public async Task NonBearerAuthorizationHeader_ReturnsNull()
    {
        var (provider, _) = Create();
        var context = new DefaultHttpContext();
        context.Request.Headers.Authorization = "Basic dXNlcjpwYXNz";

        var user = await provider.GetAuthenticatedUserAsync(context);

        Assert.Null(user);
    }

    [Fact]
    public void ConfigureOidc_ValidOptions_RegistersBoundaryServices()
    {
        var (provider, _) = Create();
        var builder = WebApplication.CreateBuilder();

        provider.ConfigureOidc(builder, Options());

        Assert.Contains(builder.Services, s => s.ServiceType == typeof(IAuthProvider));
        Assert.Contains(builder.Services, s => s.ServiceType == typeof(KeycloakClientOptions));
    }

    [Theory]
    [InlineData("", "client", "/signin-oidc")]        // empty authority
    [InlineData("not-a-uri", "client", "/signin-oidc")] // malformed authority
    [InlineData("https://issuer.test", "", "/signin-oidc")] // empty client id
    [InlineData("https://issuer.test", "client", "signin-oidc")] // callback path without leading '/'
    public void ConfigureOidc_InvalidOptions_Throws(string authority, string clientId, string callbackPath)
    {
        var (provider, _) = Create();
        var builder = WebApplication.CreateBuilder();

        Assert.Throws<OidcConfigurationException>(() => provider.ConfigureOidc(builder,
            new KeycloakClientOptions(authority, clientId, callbackPath)));
    }

    [Fact]
    public async Task Roles_ExtractedVerbatim_NeverInvented()
    {
        var (provider, issuer) = Create();
        var token = issuer.IssueToken("u001", [RoleNames.Employee, RoleNames.HrAdministrator, "office-manager"],
            DateTimeOffset.UtcNow.AddHours(1));

        var user = await provider.GetAuthenticatedUserAsync(ContextWithBearer(token));

        Assert.NotNull(user);
        Assert.Equal(3, user.Roles.Count); // verbatim extraction — no renaming, no invention
        Assert.Contains("office-manager", user.Roles);
    }

    [Fact]
    public async Task TokenWithoutRoles_YieldsEmptyRoleSet()
    {
        var (provider, issuer) = Create();
        var token = issuer.IssueToken("u001", [], DateTimeOffset.UtcNow.AddHours(1));

        var user = await provider.GetAuthenticatedUserAsync(ContextWithBearer(token));

        Assert.NotNull(user);
        Assert.Empty(user.Roles);
    }

    // ---- white-box: failure seams and input validation ----

    [Fact]
    public async Task JwksUnreachable_RejectedWithInnerException()
    {
        var issuer = new StubOidcIssuer(Authority, ClientId);
        var provider = new KeycloakAuthProvider(issuer, new UnreachableJwksProvider(), Options());
        var token = issuer.IssueToken("u001", [RoleNames.Employee], DateTimeOffset.UtcNow.AddHours(1));

        var exception = await Assert.ThrowsAsync<OidcTokenValidationException>(() => provider.GetAuthenticatedUserAsync(ContextWithBearer(token)));
        Assert.NotNull(exception.InnerException);
    }

    [Fact]
    public async Task MalformedJwks_Rejected()
    {
        var issuer = new StubOidcIssuer(Authority, ClientId);
        var provider = new KeycloakAuthProvider(issuer, new MalformedJwksProvider(), Options());
        var token = issuer.IssueToken("u001", [RoleNames.Employee], DateTimeOffset.UtcNow.AddHours(1));

        await Assert.ThrowsAsync<OidcTokenValidationException>(() => provider.GetAuthenticatedUserAsync(ContextWithBearer(token)));
    }

    [Fact]
    public async Task EmptyAuthorizationCode_Throws()
    {
        var (provider, _) = Create();

        await Assert.ThrowsAsync<OidcTokenValidationException>(() => provider.HandleOidcCallbackAsync(""));
    }

    [Fact]
    public async Task UnknownAuthorizationCode_Throws()
    {
        var (provider, _) = Create();

        await Assert.ThrowsAsync<OidcTokenValidationException>(() => provider.HandleOidcCallbackAsync("never-issued"));
    }

    [Fact]
    public void BuildAuthorizeRedirectUrl_EmptyInputs_Throw()
    {
        var (provider, _) = Create();

        Assert.Throws<ArgumentException>(() => provider.BuildAuthorizeRedirectUrl("", "state"));
        Assert.Throws<ArgumentException>(() => provider.BuildAuthorizeRedirectUrl("/cb", ""));
    }

    // ---- the request boundary (OidcMiddleware per-request branches) ----

    [Fact]
    public async Task Middleware_UnauthenticatedRequest_RedirectsToIssuer()
    {
        var (provider, _) = Create();
        var invoked = false;
        var middleware = new OidcMiddleware(_ => { invoked = true; return Task.CompletedTask; }, provider, Options());
        var context = new DefaultHttpContext();
        context.Request.Path = "/";

        await middleware.InvokeAsync(context);

        Assert.False(invoked); // SEC-003: the page never renders unauthenticated
        Assert.Equal(StatusCodes.Status302Found, context.Response.StatusCode);
        Assert.NotNull(context.Response.Headers.Location);
        Assert.StartsWith(Authority + "/protocol/openid-connect/auth", context.Response.Headers.Location.ToString());
    }

    [Fact]
    public async Task Middleware_InvalidToken_Returns401_NextNotInvoked()
    {
        var (provider, issuer) = Create();
        var token = issuer.IssueToken("u001", [RoleNames.Employee], DateTimeOffset.UtcNow.AddHours(-1)); // expired
        var invoked = false;
        var middleware = new OidcMiddleware(_ => { invoked = true; return Task.CompletedTask; }, provider, Options());

        await middleware.InvokeAsync(ContextWithBearer(token));

        Assert.False(invoked); // rejected at the boundary
        Assert.Equal(StatusCodes.Status401Unauthorized, ((DefaultHttpContext)ContextWithBearer(token)).Response.StatusCode);
    }

    [Fact]
    public async Task Middleware_ValidToken_SetsUserAndContinues()
    {
        var (provider, issuer) = Create();
        var token = issuer.IssueToken("u001", [RoleNames.Employee], DateTimeOffset.UtcNow.AddHours(1));
        var invoked = false;
        var middleware = new OidcMiddleware(_ => { invoked = true; return Task.CompletedTask; }, provider, Options());
        var context = ContextWithBearer(token);

        await middleware.InvokeAsync(context);

        Assert.True(invoked);
        Assert.True(context.Items.ContainsKey(OidcMiddleware.AuthenticatedUserItemKey));
    }

    [Fact]
    public async Task Middleware_CallbackWithCode_AuthenticatesAndContinues()
    {
        var (provider, issuer) = Create();
        var code = issuer.IssueAuthorizationCode("u001", [RoleNames.Employee], DateTimeOffset.UtcNow.AddHours(1));
        var invoked = false;
        var middleware = new OidcMiddleware(_ => { invoked = true; return Task.CompletedTask; }, provider, Options());
        var context = new DefaultHttpContext();
        context.Request.Path = CallbackPath;
        context.Request.QueryString = new QueryString("?code=" + code);

        await middleware.InvokeAsync(context);

        Assert.True(invoked);
        var user = Assert.IsType<AuthenticatedUser>(context.Items[OidcMiddleware.AuthenticatedUserItemKey]);
        Assert.Equal("u001", user.Uid);
    }

    [Fact]
    public async Task Middleware_CallbackWithoutCode_Returns400()
    {
        var (provider, _) = Create();
        var invoked = false;
        var middleware = new OidcMiddleware(_ => { invoked = true; return Task.CompletedTask; }, provider, Options());
        var context = new DefaultHttpContext();
        context.Request.Path = CallbackPath;

        await middleware.InvokeAsync(context);

        Assert.False(invoked);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    private sealed class UnreachableJwksProvider : IJwksProvider
    {
        public Task<string> GetJwksJsonAsync(string issuer)
            => throw new HttpRequestException("The issuer is unreachable.");
    }

    private sealed class MalformedJwksProvider : IJwksProvider
    {
        public Task<string> GetJwksJsonAsync(string issuer) => Task.FromResult("this-is-not-json");
    }
}
