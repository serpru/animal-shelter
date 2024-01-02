using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTest.Api.Test
{
    public static class TestClient
    {
        public static HttpClient GetTestClient()
        {
            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    //services.AddMvc(options =>
                    //{
                    //    options.Filters.Add(new AllowAnonymousFilter());
                    //    options.Filters.Add(new FakeUserFilter());
                    //}).AddApplicationPart(typeof(Program).Assembly);
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthHandler.DefaultScheme;
                    }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        TestAuthHandler.DefaultScheme, options => { });
                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                });
            }).CreateClient();
        }
    }
}