using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using pieskibackend.Api.Enums;
using pieskibackend.Api.Requests;
using pieskibackend.Models;
using pieskibackend.Models.Dictionaries.Db;
using pieskibackend.Models.Dictionaries.Responses;
using pieskibackend.Models.Dictionaries.Shorts;
using System.Threading.Tasks.Dataflow;
using TodoApi.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace pieskibackend
{
    public static class ApiEndpoints
    {
        public static void Configure(WebApplication app)
        {
            app.MapPost("/adoptee-add", (MyDatabase db, [FromBody] AdopteeAddRequest request) => {
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
            app.MapGet("/adoption/{id}", (MyDatabase db, int id) =>
            {
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

            app.MapPost("/adoption-add", (MyDatabase db, [FromBody] AdoptionEventAddRequest request) => {
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
            app.MapPost("/adoption-edit/{id}", (MyDatabase db, [FromBody] AdoptionEventEditRequest request, int id) => {
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
            app.MapGet("/adoption-form-data", (MyDatabase db) => {
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
            app.MapGet("/adoption-status", (MyDatabase db) =>
            {
                var statuses = db.AdoptionStatus.ToList();

                if (statuses == null)
                {
                    return Results.NotFound("No statuses found");
                }

                return Results.Ok(statuses);
            });
            app.MapGet("/animals", async (MyDatabase db, int page, int size, string? sort, string? qp) => {

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
                
                return new PaginatedResponse<IEnumerable<Animal>>()
                {
                    Page = page,
                    Size = size,
                    TotalElements = animals.Count,
                    Data = animals
                    .Skip(page * size)
                    .Take(size)
                };
            });

            app.MapGet("/animal/{id}", (MyDatabase db, int id) => {
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

            app.MapGet("/animal-form-data", (MyDatabase db) => {

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

            app.MapPost("/animal-add", (MyDatabase db, [FromBody] AnimalAddRequest request) => {
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

            app.MapPost("/animal-edit/{id}", (MyDatabase db, [FromBody] AnimalEditRequest request, int id) => {
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

            app.MapGet("/species", (MyDatabase db) => {
                var species = db.AnimalSpecies.ToList();

                if (species == null)
                {
                    return Results.NotFound("No species found");
                }

                return Results.Ok(species);
            });

            app.MapGet("/breeds", (MyDatabase db) => {
                var breeds = db.Breed.ToList();

                if (breeds == null)
                {
                    return Results.NotFound("No breeds found");
                }

                return Results.Ok(breeds);
            });

            app.MapPost("/login", (MyDatabase db, [FromBody] LoginRequest body) => {
                var user = db.User.FirstOrDefault(x => x.Login == body.Login && x.Password == body.Password);
                if (user is null)
                {
                    return Results.BadRequest("Skill issue :/");
                }
                return Results.Ok("Hepi fok");
                
            });

            app.MapGet("/employee/{id}", (MyDatabase db, int id) => {
                var employee = db.Employee
                .Include(x => x.JobType)
                .FirstOrDefault(x => x.Id == id);

                if (employee == null)
                {
                    return Results.NotFound("No employee found");
                }

                return Results.Ok(employee);
            });

            app.MapGet("/employees", async (MyDatabase db, int page, int size, string? sort, string? qp) =>
            {
                var employees = await db.Employee
                .Include(x => x.JobType)
                .ToListAsync();

                if (!string.IsNullOrEmpty(qp)) { 
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

                return new PaginatedResponse<IEnumerable<Employee>>()
                {
                    Page = page,
                    Size = size,
                    TotalElements = employees.Count,
                    Data = employees
                    .Skip(page * size)
                    .Take(size)
                };
            });

            app.MapPost("/employee-add", (MyDatabase db, [FromBody] EmployeeAddRequest request) => {
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

            app.MapPost("/employee-edit/{id}", (MyDatabase db, [FromBody] EmployeeEditRequest request, int id) => {
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

            app.MapGet("/employee-form-data", (MyDatabase db) => {

                var jobTypes = db.JobType.ToList();

                EmployeeAddForm data = new EmployeeAddForm(jobTypes);

                if (data == null)
                {
                    return Results.NotFound("No data found");
                }

                return Results.Ok(data);
            });
            app.MapGet("/visit/{id}", (MyDatabase db, int id) =>
            {
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
            app.MapPost("/visit-add", (MyDatabase db, [FromBody] VisitAddRequest request) => {
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
            app.MapPost("/visit-edit/{id}", (MyDatabase db, [FromBody] VisitEditRequest request, int id) => {
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
            app.MapGet("/visit-form-data", (MyDatabase db) => {
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


        }
    }
}
