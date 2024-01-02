using Azure;
using Azure.Core;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using pieskibackend.Api.Enums;
using pieskibackend.Api.Requests;
using pieskibackend.Models;
using pieskibackend.Models.Dictionaries.Db;
using pieskibackend.Models.Dictionaries.Responses;
using pieskibackend.Models.Dictionaries.Shorts;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks.Dataflow;
using TodoApi.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace pieskibackend
{
    public static class ApiEndpoints
    {
        public static string GenerateToken(IConfiguration config, User user)
        {
            var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Login),
                    new Claim(JwtClaimTypes.Name, user.Login),
                    new Claim(JwtClaimTypes.Role, user.Role.Description),
                    new Claim(ClaimTypes.Role, user.Role.Description)
                };
            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(Convert.ToDouble(config["Jwt:ATExpireSec"])),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),
                    SecurityAlgorithms.HmacSha256)
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static void SetRefreshToken(string token, HttpContext cxt)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = new JwtSecurityTokenHandler().ReadJwtToken(token).ValidTo,
                Path = "/",
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None
            };

            cxt.Response.Cookies.Append("token", token, cookieOptions);
        }
        private static bool ValidateUser(HttpContext cxt, MyDatabase db, string authorization)
        {
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var identity = cxt.User.Identity as ClaimsIdentity;
                var userName = identity.FindFirst("name").Value;

                var user = db.User.FirstOrDefault(x => x.Login == userName);
                if (user == null) { return false; }

                var scheme = headerValue.Scheme;
                var parameter = headerValue.Parameter;
                //var token = new JwtSecurityTokenHandler().ReadJwtToken(parameter);

                if (user.AccessToken != scheme)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        public static void Configure(WebApplication app)
        {
            app.MapPost("/register",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
            (MyDatabase db, HttpContext cxt, [FromBody] RegisterRequest body) =>
                {
                    var existingUser = db.User.Any(u => u.Login == body.Login);

                    if (existingUser)
                    {
                        return Results.BadRequest("User already exists.");
                    }

                    CreatePasswordHash(body.Password, out byte[] passwordHash, out byte[] passwordSalt);

                    var user = new User
                    {
                        Login = body.Login,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        Role = db.UserRole.FirstOrDefault(r => r.Id == body.RoleID),
                    };

                    db.User.Add(user);
                    db.SaveChanges();
                    return Results.Ok("User successfully created!");
                });

            //app.MapPost("/login",

            //    (MyDatabase db, HttpContext cxt, [FromBody] LoginRequest body) => {
            //    var user = db.User
            //    .Include(x => x.Role)
            //    .FirstOrDefault(x => x.Login == body.Login && x.Password == body.Password);
            //    if (user is null)
            //    {
            //        return Results.BadRequest("No user found.");
            //    }

            //    var tokenString = GenerateToken(app, user);
            //    SetRefreshToken(tokenString, cxt);
            //    user.AccessToken = tokenString;
            //    db.User.Update(user);
            //    db.SaveChanges();
            //    return Results.Ok("Logged in");
            //});
            app.MapPost("/login",

                (MyDatabase db, HttpContext cxt, [FromBody] LoginRequest body) =>
                {
                    var user = db.User
                    .Include(x => x.Role)
                    .FirstOrDefault(u => u.Login == body.Login);

                    if (user is null)
                    {
                        return Results.BadRequest("User not found");
                    }

                    if (!VerifyPasswordHash(body.Password, user.PasswordHash, user.PasswordSalt))
                    {
                        return Results.BadRequest("Login and/or password are incorrect.");
                    }

                    var tokenString = GenerateToken(app.Configuration, user);
                    SetRefreshToken(tokenString, cxt);
                    user.AccessToken = tokenString;
                    db.User.Update(user);
                    db.SaveChanges();

                    return Results.Ok("Logged in");
                });
            app.MapPost("/logout",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (MyDatabase db, HttpContext cxt) =>
                {
                    var token = cxt.Request.Cookies["token"];
                    if (token != null)
                    {
                        var user = db.User.FirstOrDefault(x => x.AccessToken == token);
                        if (user != null)
                        {
                            user.AccessToken = null;
                        }
                        else
                        {
                            return Results.BadRequest("User and token do not match");
                        }

                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            Expires = DateTime.Now.AddDays(-7),
                            Path = "/",
                            SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None
                        };
                        cxt.Response.Cookies.Append("token", "", cookieOptions);
                        //cxt.Response.Cookies.Delete("token");


                        db.User.Update(user);
                        db.SaveChanges();
                        return Results.Ok("Logout");
                    }

                    return Results.BadRequest("Token does not exist.");
                });
            app.MapGet("/login-check",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (MyDatabase db, HttpContext cxt) =>
                {
                    var response = false;
                    var token = cxt.Request.Cookies["token"];
                    if (token != null)
                    {
                        var user = db.User.FirstOrDefault(x => x.AccessToken == token);
                        if (user == null)
                        {
                            return Results.BadRequest("User and token do not match");
                        }
                        response = true;

                        return Results.Ok(response);
                    }

                    return Results.Ok(response);
                });
            app.MapPost("/adoptee-add",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db, [FromBody] AdopteeAddRequest request) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var response = request.MapToAdoptee(db);

                if (response.Status == ResponseStatus.Error)
                {
                    return Results.BadRequest(response.Message);
                }

                if (response.Status == ResponseStatus.Success)
                {
                    db.Adoptee.Add(response.Data);
                    db.SaveChanges();
                    return Results.Ok(response.Message);
                }

                return Results.BadRequest("Bad request");

            });
            app.MapGet("/adoption/{id}",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db, int id) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var adoption = db.AdoptionEvent
                .Include(x => x.Adoptee)
                .Include(x => x.Employee)
                    .Include(x => x.Employee.JobType)
                .Include(x => x.AdoptionStatus)
                .Include(x => x.Animal)
                    .Include(x => x.Animal.AggressionAnimals)
                    .Include(x => x.Animal.AggressionHumans)
                    .Include(x => x.Animal.Breed)
                        .Include(x => x.Animal.Breed.AnimalSpecies)
                    .Include(x => x.Animal.Origin)
                    .Include(x => x.Animal.Status)
                .FirstOrDefault(x => x.Id == id);

                var visits = db.Visit
                    .Include(x => x.VisitStatus)
                    .Where(x => x.AdoptionEventId == adoption.Id)
                    .ToList();

                var adoptionWithVisits = new AdoptionWithVisits(adoption, visits);

                if (adoption == null)
                {
                    return Results.NotFound("No adoption found");
                }

                return Results.Ok(adoptionWithVisits);
            });

            app.MapPost("/adoption-add",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db, [FromBody] AdoptionEventAddRequest request) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var response = request.MapToAdoptionEvent(db);

                if (response.Status == ResponseStatus.Error)
                {
                    return Results.BadRequest(response.Message);
                }

                if (response.Status == ResponseStatus.Success)
                {
                    db.AdoptionEvent.Add(response.Data);
                    db.SaveChanges();
                    return Results.Ok(response.Message);
                }

                return Results.BadRequest("Bad request");

            });
            app.MapPost("/adoption-edit/{id}",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db, [FromBody] AdoptionEventEditRequest request, int id) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var response = request.MapToAdoptionEvent(db);

                if (response.Status == ResponseStatus.Error)
                {
                    return Results.BadRequest(response.Message);
                }

                if (response.Status == ResponseStatus.Success)
                {
                    db.AdoptionEvent.Update(response.Data);
                    db.SaveChanges();
                    return Results.Ok(response.Message);
                }

                return Results.BadRequest("Bad request");

            });
            app.MapGet("/adoption-form-data",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var adoptees = db.Adoptee
                .Select(x => new AdopteeShort(x.Id, x.FirstName, x.LastName))
                .ToList();
                var employees = db.Employee
                .Select(x => new EmployeeShort(x.Id, x.FirstName, x.LastName))
                .ToList();
                var adoptionStatuses = db.AdoptionStatus.ToList();
                var animals = db.Animal.Include(x => x.Breed.AnimalSpecies)
                    .Where(x => x.Status.Id == (int)AnimalStatus.Available)
                    .Select(x => new AnimalShort(x.Id, x.Name, x.Breed, x.BirthDate))
                    .ToList();

                AdoptionAddForm data = new AdoptionAddForm(adoptees, employees, adoptionStatuses, animals);

                if (data == null)
                {
                    return Results.NotFound("No data found");
                }

                return Results.Ok(data);
            });
            app.MapGet("/adoption-status",
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db) =>
            {
                var token = cxt.Request.Cookies["token"];
                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var statuses = db.AdoptionStatus.ToList();

                if (statuses == null)
                {
                    return Results.NotFound("No statuses found");
                }

                return Results.Ok(statuses);
            });

            app.MapGet("/animals",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            async (HttpContext cxt, MyDatabase db, int page, int size, string? sort, string? qp) =>
                {
                    var token = cxt.Request.Cookies["token"];

                    if (!ValidateUser(cxt, db, token))
                    {
                        return Results.Forbid();
                    }
                    var animals = await db.Animal
                    .Include(x => x.AggressionAnimals)
                    .Include(x => x.AggressionHumans)
                    .Include(x => x.Breed)
                        .Include(x => x.Breed.AnimalSpecies)
                    .Include(x => x.Origin)
                    .Include(x => x.Status)
                    .ToListAsync();

                    if (!string.IsNullOrEmpty(qp)) { animals = animals.FindAll(x => x.Name.Contains(qp, StringComparison.InvariantCultureIgnoreCase)).ToList(); }

                    if (string.IsNullOrEmpty(sort))
                    { animals = animals.OrderBy(x => x.Name).ToList(); }
                    else if (sort.Contains("Id")) { animals = animals.OrderBy(x => x.Id).ToList(); }
                    else if (sort.Contains("Name")) { animals = animals.OrderBy(x => x.Name).ToList(); }
                    else if (sort.Contains("Age")) { animals = animals.OrderBy(x => x.BirthDate).ThenBy(x => x.Name).ToList(); }


                    var results = new PaginatedResponse<IEnumerable<Animal>>()
                    {
                        Page = page,
                        Size = size,
                        TotalElements = animals.Count,
                        Data = animals
                        .Skip(page * size)
                        .Take(size)
                    };
                    return Results.Ok(results);
                });

            app.MapGet("/animal/{id}",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db, int id) =>
                {
                    var token = cxt.Request.Cookies["token"];

                    if (!ValidateUser(cxt, db, token))
                    {
                        return Results.Forbid();
                    }

                    var animal = db.Animal
                    .Include(x => x.AggressionAnimals)
                    .Include(x => x.AggressionHumans)
                    .Include(x => x.Breed)
                        .Include(x => x.Breed.AnimalSpecies)
                    .Include(x => x.Origin)
                    .Include(x => x.Status)
                    .FirstOrDefault(x => x.Id == id);

                    if (animal == null)
                    {
                        return Results.NotFound("No animal found");
                    }

                    var adoptions = db.AdoptionEvent
                        .Where(x => x.Animal.Id == id)
                        .Select(x => new AdoptionShort(
                            x.Id,
                            $"{x.Adoptee.FirstName} {x.Adoptee.LastName}",
                            x.AdoptionStatus,
                            x.StartDate,
                            x.EndDate,
                            x.Animal.Name,
                            x.Note))
                        .ToList();

                    var response = new AnimalResponse(animal, adoptions);

                    return Results.Ok(response);
                });

            app.MapGet("/animal-form-data",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var aggressions = db.Aggression.ToList();
                var breeds = db.Breed.Include(x => x.AnimalSpecies).ToList();
                var sizes = db.Size.ToList();
                var origins = db.Origin.ToList();
                var statuses = db.Status.ToList();

                AnimalAddForm data = new AnimalAddForm(
                    aggressions, breeds, sizes, origins, statuses);


                if (data == null)
                {
                    return Results.NotFound("No data found");
                }

                return Results.Ok(data);
            });

            app.MapPost("/animal-add",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db, [FromBody] AnimalAddRequest request) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var response = request.MapToAnimal(db);

                if (response.Status == ResponseStatus.Error)
                {
                    return Results.BadRequest(response.Message);
                }

                if (response.Status == ResponseStatus.Success)
                {
                    db.Animal.Add(response.Data);
                    db.SaveChanges();
                    return Results.Ok(response.Message);
                }

                return Results.BadRequest("Bad request");

            });

            app.MapPost("/animal-edit/{id}",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db, [FromBody] AnimalEditRequest request, int id) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var response = request.MapToAnimal(db);

                if (response.Status == ResponseStatus.Error)
                {
                    return Results.BadRequest(response.Message);
                }

                if (response.Status == ResponseStatus.Success)
                {
                    db.Animal.Update(response.Data);
                    db.SaveChanges();
                    return Results.Ok(response.Message);
                }

                return Results.BadRequest("Bad request");

            });

            app.MapGet("/species",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db) =>
            {
                var token = cxt.Request.Cookies["token"];

                //if (!ValidateUser(cxt, db, token))
                //{
                //    return Results.Forbid();
                //}
                var species = db.AnimalSpecies.ToList();

                if (species == null)
                {
                    return Results.NotFound("No species found");
                }

                return Results.Ok(species);
            });

            app.MapGet("/breeds",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var breeds = db.Breed
                .Include(x => x.AnimalSpecies)
                .ToList();

                if (breeds == null)
                {
                    return Results.NotFound("No breeds found");
                }

                return Results.Ok(breeds);
            });

            app.MapGet("/employee/{id}",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db, int id) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var employee = db.Employee
                .Include(x => x.JobType)
                .FirstOrDefault(x => x.Id == id);

                if (employee == null)
                {
                    return Results.NotFound("No employee found");
                }

                return Results.Ok(employee);
            });

            app.MapGet("/employees",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            async (HttpContext cxt, MyDatabase db, int page, int size, string? sort, string? qp) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var employees = await db.Employee
                .Include(x => x.JobType)
                .ToListAsync();

                if (!string.IsNullOrEmpty(qp))
                {
                    var employeesQueried = employees.FindAll(x => x.LastName.Contains(qp, StringComparison.InvariantCultureIgnoreCase)).ToList();
                    employeesQueried.AddRange(employees.FindAll(x => x.FirstName.Contains(qp, StringComparison.InvariantCultureIgnoreCase)).ToList());
                    employees = employeesQueried;
                }

                if (string.IsNullOrEmpty(sort))
                { employees = employees.OrderBy(x => x.FirstName).ToList(); }
                else if (sort.Contains("Id")) { employees = employees.OrderBy(x => x.Id).ToList(); }
                else if (sort.Contains("First name")) { employees = employees.OrderBy(x => x.FirstName).ToList(); }
                else if (sort.Contains("Last name")) { employees = employees.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList(); }
                else if (sort.Contains("Job type")) { employees = employees.OrderBy(x => x.JobType.Description).ThenBy(x => x.LastName).ToList(); }

                var results = new PaginatedResponse<IEnumerable<Employee>>()
                {
                    Page = page,
                    Size = size,
                    TotalElements = employees.Count,
                    Data = employees
                    .Skip(page * size)
                    .Take(size)
                };
                return Results.Ok(results);
            });

            app.MapPost("/employee-add",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db, [FromBody] EmployeeAddRequest request) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var response = request.MapToEmployee(db);

                if (response.Status == ResponseStatus.Error)
                {
                    return Results.BadRequest(response.Message);
                }

                if (response.Status == ResponseStatus.Success)
                {
                    db.Employee.Add(response.Data);
                    db.SaveChanges();
                    return Results.Ok(response.Message);
                }

                return Results.BadRequest("Bad request");

            });

            app.MapPost("/employee-edit/{id}",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db, [FromBody] EmployeeEditRequest request, int id) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var response = request.MapToEmployee(db);

                if (response.Status == ResponseStatus.Error)
                {
                    return Results.BadRequest(response.Message);
                }

                if (response.Status == ResponseStatus.Success)
                {
                    db.Employee.Update(response.Data);
                    db.SaveChanges();
                    return Results.Ok(response.Message);
                }

                return Results.BadRequest("Bad request");

            });

            app.MapGet("/employee-form-data",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var jobTypes = db.JobType.ToList();

                EmployeeAddForm data = new EmployeeAddForm(jobTypes);

                if (data == null)
                {
                    return Results.NotFound("No data found");
                }

                return Results.Ok(data);
            });
            app.MapGet("/visit/{id}",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db, int id) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var visit = db.Visit
                .Include(x => x.VisitStatus)
                .FirstOrDefault(x => x.Id == id);

                var test = visit.Date.ToUniversalTime().ToString();

                if (visit == null)
                {
                    return Results.NotFound("No adoption found");
                }

                return Results.Ok(visit);
            });
            app.MapPost("/visit-add",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db, [FromBody] VisitAddRequest request) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var response = request.MapToVisit(db);

                if (response.Status == ResponseStatus.Error)
                {
                    return Results.BadRequest(response.Message);
                }

                if (response.Status == ResponseStatus.Success)
                {
                    db.Visit.Add(response.Data);
                    db.SaveChanges();
                    return Results.Ok(response.Message);
                }

                return Results.BadRequest("Bad request");

            });
            app.MapPost("/visit-edit/{id}",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db, [FromBody] VisitEditRequest request, int id) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var response = request.MapToVisit(db);

                if (response.Status == ResponseStatus.Error)
                {
                    return Results.BadRequest(response.Message);
                }

                if (response.Status == ResponseStatus.Success)
                {
                    db.Visit.Update(response.Data);
                    db.SaveChanges();
                    return Results.Ok(response.Message);
                }

                return Results.BadRequest("Bad request");

            });
            app.MapGet("/visit-form-data",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var adoptions = db.AdoptionEvent.Where(
                    x => x.AdoptionStatus.Id != (int)Api.Enums.AdoptionStatus.Fail &&
                    x.AdoptionStatus.Id != (int)Api.Enums.AdoptionStatus.Adopted)
                .Select(x => new AdoptionShort(
                    x.Id,
                    $"{x.Adoptee.FirstName} {x.Adoptee.LastName}",
                    x.AdoptionStatus,
                    x.StartDate,
                    x.EndDate,
                    x.Animal.Name,
                    x.Note
                    ))
                .ToList();


                var visitStatuses = db.VisitStatus.ToList();

                VisitAddForm data = new VisitAddForm(adoptions, visitStatuses);

                if (data == null)
                {
                    return Results.NotFound("No data found");
                }

                return Results.Ok(data);
            });
            app.MapGet("/visit-status",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER, ADMIN")]
            (HttpContext cxt, MyDatabase db) =>
            {
                var token = cxt.Request.Cookies["token"];

                if (!ValidateUser(cxt, db, token))
                {
                    return Results.Forbid();
                }
                var visitStatuses = db.VisitStatus.ToList();

                if (visitStatuses == null)
                {
                    return Results.NotFound("No data found");
                }

                return Results.Ok(visitStatuses);
            });
            app.MapGet("/work-shift/{id}", (MyDatabase db, int id) =>
            {
                var workShift = db.WorkShift
                .Include(x => x.Employee)
                .FirstOrDefault(x => x.Id == id);

                if (workShift == null)
                {
                    return Results.NotFound("No adoption found");
                }

                return Results.Ok(workShift);
            });

        }
    }
}
