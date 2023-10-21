using FirebaseAdmin.Auth;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Backend.Auth;

public class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public FirebaseAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Get the Authorization header
        if (!Request.Headers.TryGetValue("Authorization", out var authorization))
        {
            return AuthenticateResult.Fail("Cannot read authorization header.");
        }

        var idToken = authorization.ToString().Split(' ').Last();

        try
        {
            // Validate the ID token
            var decodedToken = await FirebaseAuth.DefaultInstance
                .VerifyIdTokenAsync(idToken);

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, decodedToken.Uid)
                // Add other claims as needed
            };

            // Create claims identity
            var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);

            // Create authentication ticket
            var authenticationTicket = new AuthenticationTicket(
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties(),
                Scheme.Name);

            return AuthenticateResult.Success(authenticationTicket);
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail(ex.Message);
        }
    }
}
