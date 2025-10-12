using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Shared.Security.Authentication
{
    /// <summary>
    /// Development authentication handler that allows requests without authentication
    /// ONLY FOR LOCAL DEVELOPMENT - NEVER USE IN PRODUCTION
    /// </summary>
    public class DevAuthenticationOptions : AuthenticationSchemeOptions
    {
    }

    public class DevAuthenticationHandler : AuthenticationHandler<DevAuthenticationOptions>
    {
        public DevAuthenticationHandler(
            IOptionsMonitor<DevAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Create a default development user with all roles for testing
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "dev-user-123"),
                new Claim(ClaimTypes.Name, "Development User"),
                new Claim(ClaimTypes.Email, "dev@healthcare.local"),
                new Claim(ClaimTypes.Role, "Patient"),
                new Claim(ClaimTypes.Role, "HospitalStaff"),
                new Claim(ClaimTypes.Role, "SystemAdmin")
            };

            var identity = new ClaimsIdentity(claims, "Development");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Development");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
