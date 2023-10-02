using Microsoft.EntityFrameworkCore;
using pieskibackend.Models;
using pieskibackend.Models.Dictionaries;
using System.Text.Json.Serialization;
using TodoApi.Models;

namespace pieskibackend.Api.Requests
{
    public class BreedRequest
    {

    }
    public class AnimalAddRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("birth_date")]
        public DateTime BirthDate { get; set; }
        [JsonPropertyName("aggression_animals_id")]
        public int AggressionAnimalsId { get; set; }
        [JsonPropertyName("aggression_humans_id")]
        public int AggressionHumansId { get; set; }
        [JsonPropertyName("note")]
        public string Note { get; set; }
        [JsonPropertyName("breed_id")]
        public int BreedId { get; set; }
        //[JsonPropertyName("animal_species_id")]
        //public int AnimalSpeciesId { get; set; }
        [JsonPropertyName("weight_kg")]
        public double WeightKg { get; set; }
        [JsonPropertyName("need_medication")]
        public bool NeedMedication { get; set; }
        [JsonPropertyName("origin_id")]
        public int OriginId { get; set; }
        [JsonPropertyName("status_id")]
        public int StatusId { get; set; }
        [JsonPropertyName("arrive_date")]
        public DateTime ArriveDate { get; set; }
        [JsonPropertyName("adoption_date")]
        public DateTime? AdoptionDate { get; set; }

        public ResponseWrapper<Animal> MapToAnimal(MyDatabase db)
        {
            var animal = new Animal(
                Name,
                BirthDate,
                db.Aggression.FirstOrDefault(a => a.Id == AggressionAnimalsId),
                db.Aggression.FirstOrDefault(a => a.Id == AggressionHumansId),
                Note,
                db.Breed.Include(x => x.AnimalSpecies).FirstOrDefault(a => a.Id == BreedId),
                WeightKg,
                NeedMedication,
                db.Origin.FirstOrDefault(a => a.Id == OriginId),
                db.Status.FirstOrDefault(a => a.Id == StatusId),
                ArriveDate,
                AdoptionDate
               );

            return new ResponseWrapper<Animal>()
            {
                Status = Enums.ResponseStatus.Success,
                Message = "Added animal to database.",
                Data = animal
            };
        }
    }
}
