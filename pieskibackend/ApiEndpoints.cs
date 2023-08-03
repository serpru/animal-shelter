using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pieskibackend.Api.Requests;
using pieskibackend.Models;
using pieskibackend.Models.Dictionaries;
using TodoApi.Models;

namespace pieskibackend
{
    public static class ApiEndpoints
    {
        public static void Configure(WebApplication app)
        {
            app.MapGet("/animals", async (MyDatabase db, int page, int size) => {
                var animals = await db.Animal
                .Include(x => x.AnimalSpecies)
                .Include(x => x.AggressionAnimals)
                .Include(x => x.AggressionHumans)
                .Include(x => x.Breed)
                .Include(x => x.Origin)
                .Include(x => x.Status)
                .ToListAsync();

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
                .Include(x => x.AnimalSpecies)
                .Include(x => x.AggressionAnimals)
                .Include(x => x.AggressionHumans)
                .Include(x => x.Breed)
                .Include(x => x.Origin)
                .Include(x => x.Status)
                .FirstOrDefault(x => x.Id == id);

                if (animal == null) {
                    return Results.NotFound("No animal found");
                }

                return Results.Ok(animal);
            });

            app.MapPost("/animal-add", (MyDatabase db, [FromBody] AnimalAddRequest request) => {
                db.Animal.Add(request.MapToAnimal(db));
                db.SaveChanges();
                
            });

            app.MapGet("/species", async (MyDatabase db) => {
                return await db.AnimalSpecies.ToListAsync();
            });

            app.MapPost("/login", (MyDatabase db, [FromBody] LoginRequest body) => {
                var user = db.User.FirstOrDefault(x => x.Login == body.Login && x.Password == body.Password);
                if (user is null)
                {
                    return Results.BadRequest("Skill issue :/");
                }
                return Results.Ok("Hepi fok");
                
            });
        }
    }
}
