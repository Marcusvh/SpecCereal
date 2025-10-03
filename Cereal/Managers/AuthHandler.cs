using Cereal.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Cereal.Managers
{
    public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IAuthManager _authManager;

        public BasicAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IAuthManager authManager) // inject your manager
            : base(options, logger, encoder, clock)
        {
            _authManager = authManager;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // checks if the Authorization header is present
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');
                var username = credentials[0];
                var password = credentials[1];

                var principal = _authManager.ValidateUser(username, password);
                if (principal == null)
                    return AuthenticateResult.Fail("Invalid Username or Password");

                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
        }
        protected override Task HandleChallengeAsync(AuthenticationProperties properties) // Override to customize the 401 response code, for swagger
        {
            Response.StatusCode = 401;
            Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new { error = "Incorrect loggin credentials" });
            return Response.WriteAsync(result);
        }
    }
}
