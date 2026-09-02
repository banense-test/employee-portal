using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EmployeePortal.Services;

namespace EmployeePortal.Infrastructure;

/// <summary>
/// INT-011 IAuthProvider (COMP-006) — the portal's OIDC client surface.
/// Postconditions: OIDC middleware registered at the request boundary; all pages require
/// authentication (SEC-003); roles mapped from claims (SEC-002).
/// </summary>
public interface IAuthProvider
{
    void ConfigureOidc(WebApplicationBuilder builder, KeycloakClientOptions options);

    /// <summary>Constructs the issuer's authorize URL — the portal's half of the redirect flow.</summary>
    string BuildAuthorizeRedirectUrl(string redirectUri, string state);

    /// <summary>Completes the redirect flow: exchanges the authorization code for a token, validates it via the issuer's JWKS, and returns the authenticated user.</summary>
    Task<AuthenticatedUser> HandleOidcCallbackAsync(string authorizationCode);

    /// <summary>Reads the identity at the request boundary: null when unauthenticated; throws OidcTokenValidationException when the presented token is expired or invalid (rejected at the boundary).</summary>
    Task<AuthenticatedUser?> GetAuthenticatedUserAsync(HttpContext context);
}

/// <summary>
/// CLS-010 KeycloakAuthProvider (COMP-006) — token validation, role extraction from claims,
/// nothing more (CON-004). R003 volatility point: Keycloak configuration nuances touch this class only.
/// </summary>
public sealed class KeycloakAuthProvider(ITokenExchangeClient tokenExchange, IJwksProvider jwks, KeycloakClientOptions options) : IAuthProvider
{
    private const string BearerPrefix = "Bearer ";
    private const string AuthorizeEndpointPath = "/protocol/openid-connect/auth";

    public void ConfigureOidc(WebApplicationBuilder builder, KeycloakClientOptions clientOptions)
    {
        clientOptions.Validate(); // INT-011 precondition: Keycloak client settings present in configuration
        builder.Services.AddSingleton(clientOptions);
        builder.Services.AddSingleton<IAuthProvider>(this);
        // SEC-003 boundary enforcement: the composition root calls app.UseMiddleware<OidcMiddleware>()
        // (ASP.NET Core registers middleware on the pipeline, not the service collection — ARCH-3).
    }

    public string BuildAuthorizeRedirectUrl(string redirectUri, string state)
    {
        if (string.IsNullOrWhiteSpace(redirectUri))
            throw new ArgumentException("The redirect URI is required.", nameof(redirectUri));
        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("The state parameter is required (CSRF protection).", nameof(state));

        var authorize = options.Authority.TrimEnd('/') + AuthorizeEndpointPath;
        return $"{authorize}?client_id={Uri.EscapeDataString(options.ClientId)}"
             + $"&redirect_uri={Uri.EscapeDataString(redirectUri)}"
             + "&response_type=code"
             + $"&state={Uri.EscapeDataString(state)}";
    }

    public async Task<AuthenticatedUser> HandleOidcCallbackAsync(string authorizationCode)
    {
        if (string.IsNullOrWhiteSpace(authorizationCode))
            throw new OidcTokenValidationException("The OIDC callback carries no authorization code.");

        var token = await tokenExchange.ExchangeCodeAsync(authorizationCode);
        return await ValidateTokenAsync(token);
    }

    public async Task<AuthenticatedUser?> GetAuthenticatedUserAsync(HttpContext context)
    {
        var token = ExtractBearerToken(context);
        if (token is null) return null; // unauthenticated — the middleware challenges; not an error
        return await ValidateTokenAsync(token);
    }

    /// <summary>
    /// Validates a signed token via the issuer's JWKS and extracts the identity + roles.
    /// Every rejection is an OidcTokenValidationException — the request boundary turns it into a 401.
    /// </summary>
    internal async Task<AuthenticatedUser> ValidateTokenAsync(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 3)
            throw new OidcTokenValidationException("Malformed token: expected three segments.");

        JsonElement header;
        try
        {
            header = JsonDocument.Parse(Base64Url.Decode(parts[0])).RootElement;
        }
        catch (FormatException ex)
        {
            throw new OidcTokenValidationException("Malformed token header.", ex);
        }

        if (!header.TryGetProperty("alg", out var alg) || alg.GetString() != "RS256")
            throw new OidcTokenValidationException($"Unsupported algorithm '{(alg.ValueKind == JsonValueKind.String ? alg.GetString() : "<missing>")}' — only RS256 is accepted.");
        if (!header.TryGetProperty("kid", out var kidElement) || kidElement.GetString() is not { Length: > 0 } kid)
            throw new OidcTokenValidationException("The token carries no key id (kid).");

        string jwksJson;
        try
        {
            jwksJson = await jwks.GetJwksJsonAsync(options.Authority);
        }
        catch (Exception ex)
        {
            throw new OidcTokenValidationException("The issuer's JWKS could not be retrieved.", ex);
        }

        VerifySignature(parts, kid, jwksJson);

        JsonElement payload;
        try
        {
            payload = JsonDocument.Parse(Base64Url.Decode(parts[1])).RootElement;
        }
        catch (FormatException ex)
        {
            throw new OidcTokenValidationException("Malformed token payload.", ex);
        }

        if (!payload.TryGetProperty("exp", out var exp) || !exp.TryGetInt64(out var expiresAtEpoch))
            throw new OidcTokenValidationException("The token carries no expiry.");
        if (expiresAtEpoch <= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            throw new OidcTokenValidationException("The token is expired.");

        if (!payload.TryGetProperty("iss", out var iss) || iss.GetString() != options.Authority)
            throw new OidcTokenValidationException("The token issuer does not match the configured authority.");

        if (!payload.TryGetProperty("aud", out var aud))
            throw new OidcTokenValidationException("The token carries no audience.");
        var audiences = aud.ValueKind == JsonValueKind.Array
            ? aud.EnumerateArray().Select(a => a.GetString()).ToList()
            : [aud.GetString()];
        if (!audiences.Contains(options.ClientId))
            throw new OidcTokenValidationException("The token audience does not include this client.");

        if (!payload.TryGetProperty("sub", out var sub) || string.IsNullOrEmpty(sub.GetString()))
            throw new OidcTokenValidationException("The token carries no subject (sub).");

        return new AuthenticatedUser(sub.GetString()!, MapRoles(payload));
    }

    private static void VerifySignature(string[] parts, string kid, string jwksJson)
    {
        JsonElement keys;
        try
        {
            keys = JsonDocument.Parse(jwksJson).RootElement.GetProperty("keys");
        }
        catch (Exception ex) when (ex is KeyNotFoundException or JsonException or FormatException)
        {
            throw new OidcTokenValidationException("The issuer's JWKS is malformed or carries no keys.", ex);
        }

        foreach (var key in keys.EnumerateArray())
        {
            if (key.ValueKind != JsonValueKind.Object) continue;
            if (!key.TryGetProperty("kid", out var keyId) || keyId.GetString() != kid) continue;
            if (!key.TryGetProperty("kty", out var keyType) || keyType.GetString() != "RSA") continue;

            var modulus = key.TryGetProperty("n", out var n) ? n.GetString() : null;
            var exponent = key.TryGetProperty("e", out var e) ? e.GetString() : null;
            if (modulus is null || exponent is null)
                throw new OidcTokenValidationException($"The JWKS key '{kid}' is missing its modulus or exponent.");

            using var rsa = RSA.Create();
            rsa.ImportParameters(new RSAParameters { Modulus = Base64Url.Decode(modulus), Exponent = Base64Url.Decode(exponent) });

            var signedBytes = Encoding.UTF8.GetBytes(parts[0] + "." + parts[1]);
            if (!rsa.VerifyData(signedBytes, Base64Url.Decode(parts[2]), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                throw new OidcTokenValidationException("Signature validation failed.");
            return;
        }

        throw new OidcTokenValidationException($"The issuer's JWKS carries no key matching kid '{kid}'.");
    }

    /// <summary>Extracts the roles from the realm_access.roles claim VERBATIM (SEC-006) — never invented, never renamed.</summary>
    private static IReadOnlySet<string> MapRoles(JsonElement payload)
    {
        var roles = new HashSet<string>(StringComparer.Ordinal);
        if (payload.TryGetProperty("realm_access", out var realmAccess)
            && realmAccess.TryGetProperty("roles", out var roleList)
            && roleList.ValueKind == JsonValueKind.Array)
        {
            foreach (var role in roleList.EnumerateArray())
            {
                var name = role.GetString();
                if (!string.IsNullOrWhiteSpace(name)) roles.Add(name);
            }
        }
        return roles;
    }

    private static string? ExtractBearerToken(HttpContext context)
    {
        var header = context.Request.Headers.Authorization.ToString();
        if (header.Length == 0) return null;
        if (!header.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase)) return null;
        var token = header[BearerPrefix.Length..].Trim();
        return token.Length == 0 ? null : token;
    }
}

/// <summary>
/// The request-boundary OIDC enforcement (SEC-003: all pages require authentication).
/// Registered by the composition root via app.UseMiddleware<OidcMiddleware>() after
/// ConfigureOidc has validated the client options and registered the services.
/// </summary>
public sealed class OidcMiddleware(RequestDelegate next, IAuthProvider authProvider, KeycloakClientOptions options)
{
    public const string AuthenticatedUserItemKey = "EmployeePortal.AuthenticatedUser";

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == options.CallbackPath)
        {
            var code = context.Request.Query["code"].ToString();
            if (code.Length == 0)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            AuthenticatedUser user;
            try
            {
                user = await authProvider.HandleOidcCallbackAsync(code);
            }
            catch (OidcTokenValidationException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized; // rejected at the boundary
                return;
            }

            context.Items[AuthenticatedUserItemKey] = user;
            await next(context);
            return;
        }

        AuthenticatedUser? authenticated;
        try
        {
            authenticated = await authProvider.GetAuthenticatedUserAsync(context);
        }
        catch (OidcTokenValidationException)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized; // expired/invalid token: rejected at the boundary
            return;
        }

        if (authenticated is not null)
        {
            context.Items[AuthenticatedUserItemKey] = authenticated;
            await next(context);
            return;
        }

        // Unauthenticated: challenge — redirect to the issuer's authorize endpoint.
        context.Response.Redirect(authProvider.BuildAuthorizeRedirectUrl(options.CallbackPath, Guid.NewGuid().ToString("N")));
    }
}
