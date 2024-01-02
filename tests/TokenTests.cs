using Castle.Core.Configuration;
using FakeItEasy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using pieskibackend;
using pieskibackend.Models.Dictionaries.Db;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace tests
{
    internal class TokenTests
    {
        private Fake<WebApplication> _app;
        private const string _hashString = "FC41F923D89E3DCC147EEC42F30004D3A8DC80B99A850C451C00BA5BE6C9DE9810A8F67B45873D6A07C854F39CC0C3A9CEB082968A795405509521E562DB4FAB";
        private const string _saltString = "603A7D6397876A059D06506EB9EC978338507C6D906BEA08B111C42E019E770D88090A8F0D34C9FED59C9A3286346DC0894771A4C6F751B5F6257AFC38D9227D999CAE9FDE100C165A59AAECCA69A200D1AC697A0E81EEE03805CD1A0C9B080120C0A06068E2298C3ECE62AEEC7491094ED6A2A1341B5813FE87639EB9E9705D";

        private User _user = new User
        {
            Id = 3,
            Login = "user",
            PasswordHash = BigInteger.Parse(_hashString, NumberStyles.HexNumber).ToByteArray(),
            PasswordSalt = BigInteger.Parse(_saltString, NumberStyles.HexNumber).ToByteArray(),
            AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6InVzZXIiLCJuYW1lIjoidXNlciIsInJvbGUiOiJVU0VSIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVVNFUiIsIm5iZiI6MTcwMDgzMTg5NSwiZXhwIjoxNzAwODMzMDk1LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MjI0LyIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTE3My8ifQ.yZJVEMeLT_MS5VfR1z47CpqHUdXo7l1nKgOUZybg86o",
            Role = new UserRole() { Id = 2, Description = "USER" },
        };
        private string _token;
        [OneTimeSetUp]
        public void Setup()
        {
            
            //_app.CallsTo(a => a.Configuration["Jwt:Issuer"]).Returns(builder.Configuration["Jwt:Issuer"]);
            //_app.CallsTo(a => a.Configuration["Jwt:Audience"]).Returns(builder.Configuration["Jwt:Audience"]);
            //_app.CallsTo(a => a.Configuration["Jwt:ATExpireSec"]).Returns(builder.Configuration["Jwt:ATExpireSec"]);
            //_app.CallsTo(a => a.Configuration["Jwt:Key"]).Returns(builder.Configuration["Jwt:Key"]);

        }

        [Test]
        public void GenerateToken_ShouldReturnTrue()
        {
            var builder = WebApplication.CreateBuilder();
            var result = ApiEndpoints.GenerateToken(builder.Configuration, _user);
            _token = result;
            Assert.IsInstanceOf<string>(result);
        }
        [Test]
        public void SetRefreshToken_ShouldReturnTrue()
        {
            //  Mock HTTP Context
            var _httpContext = new Fake<HttpContext>();
            var requestMock = new Fake<HttpRequest>();
            var headers = new Fake<IHeaderDictionary>();
            var urlReferer = A.Fake<IHeaderDictionary>().Referer;
            var userMock = new Fake<ClaimsPrincipal>();
            var identityMock = new Fake<IIdentity>();

            userMock.CallsTo(u => u.Identity).Returns(identityMock.FakedObject);
            identityMock.CallsTo(i => i.Name).Returns("user");

            //var cookies = new CookieCollection();
            //var cookie = new Cookie("token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6InVzZXIiLCJuYW1lIjoidXNlciIsInJvbGUiOiJVU0VSIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVVNFUiIsIm5iZiI6MTcwMDgyNTE1MywiZXhwIjoxNzAwODI2MzUzLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MjI0LyIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTE3My8ifQ.SC2EW8q0acYyVnOQYW3yEEKLDzVHEoup3E56bZREXt4");
            //cookies.Add(cookie);

            var cookiesMock = new Fake<IRequestCookieCollection>();

            //var cookiesMock = new Fake<IRequestCookieCollection>();

            //cookiesMock.CallsTo(c => c["token"]).Returns(_user.AccessToken);
            //_httpContext.CallsTo(ctx => ctx.Request.Cookies).Returns(cookiesMock.FakedObject);
            //requestMock.CallsTo(r => r.Cookies).Returns(cookiesMock.FakedObject);
            //userMock.CallsTo(u => u.Identity).Returns("user");

            headers.CallsTo(x => x["Referer"]).Returns(urlReferer);
            requestMock.CallsTo(r => r.Headers).Returns(headers.FakedObject);
            _httpContext.CallsTo(x => x.Request).Returns(requestMock.FakedObject);
            _httpContext.CallsTo(x => x.User).Returns(userMock.FakedObject);
            requestMock.CallsTo(r => r.Cookies).Returns(cookiesMock.FakedObject);

            ApiEndpoints.SetRefreshToken(_token, _httpContext.FakedObject);

            var cookie = _httpContext.FakedObject.Response.Cookies;
            var token = _httpContext.FakedObject.Request.Cookies["token"];
            var x = _httpContext.FakedObject.Response;
            Assert.IsNotEmpty(token);
        }
    }
}
