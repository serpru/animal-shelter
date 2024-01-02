using FakeItEasy;
using IdentityModel;
using IntegrationTest.Api.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using pieskibackend;
using pieskibackend.Api.Enums;
using pieskibackend.Models.Dictionaries.Db;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TodoApi.Models;

namespace tests
{
    public class ApiTests
    {
        private MyDatabase _dbContext; 
        private Fake<HttpContext> _httpContext;
        private HttpClient _httpClient;

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

        [OneTimeSetUp]
        public void Setup()
        {
            //  Mock DbContext
            var optionsBuilder = new DbContextOptionsBuilder<MyDatabase>()
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            _dbContext = new MyDatabase(optionsBuilder.Options);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            var status1 = new AnimalSpecies { Id = 1, Species = "Test" };
            var status2 = new AnimalSpecies { Id = 2, Species = "Test2" };

            _dbContext.AnimalSpecies.Add(status1);
            _dbContext.AnimalSpecies.Add(status2);
            _dbContext.User.Add(_user);
            _dbContext.SaveChanges();

            //  Mock HTTP Context
            _httpContext = new Fake<HttpContext>();
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

            cookiesMock.CallsTo(c => c["token"]).Returns(_user.AccessToken);
            //_httpContext.CallsTo(ctx => ctx.Request.Cookies).Returns(cookiesMock.FakedObject);
            requestMock.CallsTo(r => r.Cookies).Returns(cookiesMock.FakedObject);
            //userMock.CallsTo(u => u.Identity).Returns("user");

            headers.CallsTo(x => x["Referer"]).Returns(urlReferer);
            requestMock.CallsTo(r => r.Headers).Returns(headers.FakedObject);
            _httpContext.CallsTo(x => x.Request).Returns(requestMock.FakedObject);
            _httpContext.CallsTo(x => x.User).Returns(userMock.FakedObject);
            //_httpContext.CallsTo(x => x.Features).Returns(features);

            _httpClient = TestClient.GetTestClient();
        }
        [Test]
        public async Task GetAdoptionForm_ShouldReturnFalse()
        {
           var response = await _httpClient.GetAsync("/adoption-form-data");
           Assert.IsFalse(response.StatusCode == HttpStatusCode.Unauthorized);
        }
        [Test]
        public async Task PostAdopteeAdd_ShouldReturnFalse()
        {
            var jsonObj = "{\r\n  \"first_name\": \"string\",\r\n  \"last_name\": \"string\",\r\n  \"phone_number\": \"string\",\r\n  \"email\": \"string\",\r\n  \"address\": \"string\",\r\n  \"city\": \"string\",\r\n  \"zipcode\": \"string\",\r\n  \"country\": \"string\"\r\n}";
            var stringContent = new StringContent(jsonObj, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/adoptee-add", stringContent);
            Assert.IsFalse(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }
        [Test]
        public async Task GetAdoption_ShouldReturnFalse()
        {
            var response = await _httpClient.GetAsync("/adoption/1");
            Assert.IsFalse(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }
        [Test]
        public async Task PostLogin_ShouldReturnFalse()
        {
            var jsonObj = "{\r\n  \"login\": \"test\",\r\n  \"password\": \"test\"\r\n}";
            var stringContent = new StringContent(jsonObj, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/login", stringContent);
            Assert.IsFalse(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        //[Test]
        //public async Task Test2()
        //{
        //    var response1 = await _httpClient.GetAsync("/species");
        //    var jsonObj = "{\r\n  \"login\": \"user\",\r\n  \"password\": \"user\"\r\n}";

        //    var content = new StringContent(jsonObj);
        //    var stringContent = new StringContent(jsonObj, Encoding.UTF8, "application/json");

        //    var response = await _httpClient.PostAsync("/login", stringContent);

        //    Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);




        //    //  Arrange
        //    //var builder = A.Fake<WebApplicationBuilder>();
        //    //builder.Services.AddDbContext<MyDatabase>();
        //    //var app = builder.Build();
        //    //ApiEndpoints.Configure(app);

        //    //var status1 = new AnimalSpecies { Id = 1, Species = "Test" };
        //    //var status2 = new AnimalSpecies { Id = 2, Species = "Test2" };

        //    ////  Mock DbContext
        //    //var optionsBuilder = new DbContextOptionsBuilder<MyDatabase>()
        //    //    .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
        //    //    .UseInMemoryDatabase(Guid.NewGuid().ToString());

        //    //var dbContext = new MyDatabase(optionsBuilder.Options);

        //    //dbContext.Database.EnsureDeleted();
        //    //dbContext.Database.EnsureCreated();

        //    ////  Mock HTTP Context
        //    //var httpContextMock = new Fake<HttpContext>();
        //    //var requestMock = new Fake<HttpRequest>();
        //    //var headers = new Fake<IHeaderDictionary>();
        //    //var urlReferer = A.Fake<IHeaderDictionary>().Referer;

        //    //headers.CallsTo(x => x["Referer"]).Returns(urlReferer);
        //    //requestMock.CallsTo(r => r.Headers).Returns(headers.FakedObject);
        //    //httpContextMock.CallsTo(x => x.Request).Returns(requestMock.FakedObject);

        //    //dbContext.AnimalSpecies.Add(status1);
        //    //dbContext.AnimalSpecies.Add(status2);
        //    //dbContext.SaveChanges();

        //    //var species = dbContext.AnimalSpecies.ToList();

        //    //var builder = WebApplication.CreateBuilder();
        //    //builder.Services.AddDbContext<MyDatabase>();
        //    //var app = builder.Build();

        //    //ApiEndpoints.Configure(app);
        //    //app.Run();

        //    //var appFactory = new WebApplicationFactory<Program>();
        //    //var client = appFactory.CreateClient();
        //    //var response = await client.GetAsync("/species");

        //    ////Act

        //    ////Assert
        //    //response.EnsureSuccessStatusCode();
        //    //Assert.Equals("text/html; charset=utf-8",
        //    //response.Content.Headers.ContentType.ToString());
        //    //Assert.Pass();
        //}
    }
}
