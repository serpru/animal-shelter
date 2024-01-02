using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using pieskibackend.Models.Dictionaries.Db;
using System.Security.Claims;

namespace IntegrationTest.Api.Test
{
    public class FakePolicyEvaluator : IPolicyEvaluator
    {
        public virtual async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            var principal = new ClaimsPrincipal();

            principal.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user"),
                new Claim(JwtClaimTypes.Name, "user"),
                new Claim(JwtClaimTypes.Role, "USER"),
                new Claim(ClaimTypes.Role, "USER"),
                //new Claim("Permission", "CanViewPage"),
                //new Claim("Manager", "yes"),
                //new Claim(ClaimTypes.Role, "Administrator"),
                //new Claim(ClaimTypes.NameIdentifier, "DipNeupane")
            }, "Bearer"));
            
            return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal,
                new AuthenticationProperties(), "FakeScheme")));
        }

        public virtual async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy,
            AuthenticateResult authenticationResult, HttpContext context, object resource)
        {
            return await Task.FromResult(PolicyAuthorizationResult.Success());
        }
    }
}