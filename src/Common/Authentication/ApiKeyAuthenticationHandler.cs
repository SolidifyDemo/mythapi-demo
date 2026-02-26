using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace MythApi.Common.Authentication;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string ApiKeyHeaderName = "X-API-Key";
    
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(ApiKeyHeaderName))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var apiKey = Request.Headers[ApiKeyHeaderName].ToString();
        
        // NOTE: This implementation uses a hardcoded API key for demonstration purposes only.
        // In a production application, API keys should be:
        // - Stored securely in configuration (appsettings.json, environment variables, or Azure Key Vault)
        // - Validated against a secure data store
        // - Hashed and compared securely
        // - Rotated regularly
        
        // Check if it's an admin key (only valid key for this demo)
        var isAdmin = apiKey == "admin-key-12345";
        
        // If the key doesn't match any valid keys, fail authentication
        if (!isAdmin)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
        }
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "ApiUser"),
            new Claim(ClaimTypes.NameIdentifier, apiKey),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
