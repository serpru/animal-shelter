using IdentityModel;
using Microsoft.AspNetCore.Mvc.Filters;
using NUnit.Framework.Internal;
using System.Security.Claims;

class FakeUserFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
                new Claim(ClaimTypes.NameIdentifier, "user"),
                new Claim(JwtClaimTypes.Name, "user"),
                new Claim(JwtClaimTypes.Role, "USER"),
                new Claim(ClaimTypes.Role, "USER"),
        }));

        await next();
    }
}