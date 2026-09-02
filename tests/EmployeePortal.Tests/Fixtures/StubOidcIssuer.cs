using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EmployeePortal.Infrastructure;

namespace EmployeePortal.Tests.Fixtures;

/// <summary>
/// The R003 stub OIDC issuer (stakeholder decision, Elab Iter 1: "R003 you mock… A stub issuer is
/// enough. Do not wait on STK-004 for this and do not build it against a real realm.").
/// Signs RS256 tokens and serves its JWKS; mints valid tokens AND every failing state
/// (expired, wrong issuer, wrong audience, alg=none, unknown kid, tampered signature) so each
/// rejection branch can actually fail — a fixture that cannot fail proves nothing.
/// Retained as a reusable Construction test asset (R011 residual).
/// </summary>
public sealed class StubOidcIssuer : IJwksProvider, ITokenExchangeClient
{
    private readonly RSA _signingKey = RSA.Create(2048);
    private readonly Dictionary<string, string> _tokensByCode = new(StringComparer.Ordinal);

    public string Authority { get; }
    public string ClientId { get; }
    public string KeyId { get; } = "stub-signing-key-1";

    public StubOidcIssuer(string authority, string clientId)
    {
        Authority = authority;
        ClientId = clientId;
    }

    /// <summary>Mints a signed JWT. The overrides produce the failing states; tamperSignature corrupts one signature byte.</summary>
    public string IssueToken(
        string subject,
        IReadOnlyList<string> roles,
        DateTimeOffset expiresAtUtc,
        string? issuerOverride = null,
        string? audienceOverride = null,
        string? algorithmOverride = null,
        string? keyIdOverride = null,
        bool tamperSignature = false)
    {
        var header = new Dictionary<string, object?>
        {
            ["alg"] = algorithmOverride ?? "RS256",
            ["typ"] = "JWT",
            ["kid"] = keyIdOverride ?? KeyId,
        };
        var payload = new Dictionary<string, object?>
        {
            ["sub"] = subject,
            ["iss"] = issuerOverride ?? Authority,
            ["aud"] = audienceOverride ?? ClientId,
            ["exp"] = expiresAtUtc.ToUnixTimeSeconds(),
            ["realm_access"] = new Dictionary<string, object?> { ["roles"] = roles.ToList() },
        };

        var headerSegment = Base64Url.Encode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(header)));
        var payloadSegment = Base64Url.Encode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)));
        var signingInput = headerSegment + "." + payloadSegment;

        var signature = _signingKey.SignData(Encoding.UTF8.GetBytes(signingInput), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        if (tamperSignature) signature[0] ^= 0xFF; // corrupt one byte — signature verification must fail

        return signingInput + "." + Base64Url.Encode(signature);
    }

    /// <summary>The issuer's authorize step: simulates the user authenticating and returns an authorization code.</summary>
    public string IssueAuthorizationCode(
        string subject,
        IReadOnlyList<string> roles,
        DateTimeOffset expiresAtUtc,
        string? issuerOverride = null,
        string? audienceOverride = null,
        string? algorithmOverride = null,
        string? keyIdOverride = null,
        bool tamperSignature = false)
    {
        var token = IssueToken(subject, roles, expiresAtUtc, issuerOverride, audienceOverride, algorithmOverride, keyIdOverride, tamperSignature);
        var code = Base64Url.Encode(RandomNumberGenerator.GetBytes(16));
        _tokensByCode[code] = token;
        return code;
    }

    public Task<string> ExchangeCodeAsync(string authorizationCode)
        => Task.FromResult(_tokensByCode.TryGetValue(authorizationCode, out var token)
            ? token
            : throw new OidcTokenValidationException("Unknown authorization code."));

    public Task<string> GetJwksJsonAsync(string issuer)
    {
        var parameters = _signingKey.ExportParameters(includePrivateParameters: false);
        var jwk = new Dictionary<string, object?>
        {
            ["kty"] = "RSA",
            ["kid"] = KeyId,
            ["use"] = "sig",
            ["alg"] = "RS256",
            ["n"] = Base64Url.Encode(parameters.Modulus!),
            ["e"] = Base64Url.Encode(parameters.Exponent!),
        };
        return Task.FromResult(JsonSerializer.Serialize(new Dictionary<string, object?> { ["keys"] = new List<object?> { jwk } }));
    }
}
