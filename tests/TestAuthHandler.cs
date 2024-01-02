using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string DefaultScheme = "Test";
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] {                 
                new Claim(ClaimTypes.NameIdentifier, "user"),
                new Claim(JwtClaimTypes.Name, "user"),
                new Claim(JwtClaimTypes.Role, "USER"),
                new Claim(ClaimTypes.Role, "USER"), };
        var identity = new ClaimsIdentity(claims, DefaultScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, DefaultScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}