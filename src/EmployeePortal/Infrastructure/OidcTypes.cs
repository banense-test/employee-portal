namespace EmployeePortal.Infrastructure;

/// <summary>
/// The registered OIDC client settings (CON-004: the portal is an OIDC client — register a client,
/// redirect for login, validate the token, read roles from claims; nothing more).
/// The production values land with the Keycloak client registration (R010, Construction).
/// </summary>
public sealed record KeycloakClientOptions(string Authority, string ClientId, string CallbackPath)
{
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Authority) || !Uri.IsWellFormedUriString(Authority, UriKind.Absolute))
            throw new OidcConfigurationException("KeycloakClientOptions.Authority must be a non-empty absolute URI (the issuer).");
        if (string.IsNullOrWhiteSpace(ClientId))
            throw new OidcConfigurationException("KeycloakClientOptions.ClientId must be non-empty (the registered OIDC client id).");
        if (string.IsNullOrWhiteSpace(CallbackPath) || !CallbackPath.StartsWith('/'))
            throw new OidcConfigurationException("KeycloakClientOptions.CallbackPath must be a path starting with '/'.");
    }
}

/// <summary>Missing/invalid OIDC client configuration — thrown by ConfigureOidc (INT-011 precondition).</summary>
public sealed class OidcConfigurationException(string message) : Exception(message);

/// <summary>An expired or invalid token — the request boundary turns this into a 401 rejection.</summary>
public sealed class OidcTokenValidationException(string message, Exception? inner = null) : Exception(message, inner);

/// <summary>
/// The issuer's JWKS endpoint seam. The production adapter fetches {authority}/protocol/openid-connect/certs
/// (real Keycloak, R010 — Construction); the stub issuer serves its own JWKS for the Elaboration validation.
/// </summary>
public interface IJwksProvider
{
    Task&lt;string&gt; GetJwksJsonAsync(string issuer);
}

/// <summary>
/// The token-endpoint seam (authorization code exchange). The production adapter POSTs to the real
/// token endpoint (R010 — Construction); the stub issuer exchanges its own codes.
/// </summary>
public interface ITokenExchangeClient
{
    Task&lt;string&gt; ExchangeCodeAsync(string authorizationCode);
}

/// <summary>Base64url encoding/decoding (RFC 7515) — the JWT/JWKS wire format.</summary>
public static class Base64Url
{
    public static string Encode(byte[] bytes)
    {
        var base64 = Convert.ToBase64String(bytes).TrimEnd('=');
        return base64.Replace('+', '-').Replace('/', '_');
    }

    public static byte[] Decode(string value)
    {
        var base64 = value.Replace('-', '+').Replace('_', '/');
        return Convert.FromBase64String(base64.Length % 4 switch
        {
            2 =&gt; base64 + "==",
            3 =&gt; base64 + "=",
            _ =&gt; base64,
        });
    }
}
