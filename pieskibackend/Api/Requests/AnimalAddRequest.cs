using pieskibackend.Models;
using pieskibackend.Models.Dictionaries;
using TodoApi.Models;

namespace pieskibackend.Api.Requests
{
    public class AnimalAddRequest
    {
        public int AnimalSpeciesId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int AggressionAnimalsId { get; set; }
        public int AggressionHumansId { get; set; }
        public string Note { get; set; }
        public int BreedId { get; set; }
        public int WeightKg { get; set; }
        public int SizeId { get; set; }
        public bool NeedMedication { get; set; }
        public int OriginId { get; set; }
        public int StatusId { get; set; }
        public DateTime ArriveDate { get; set; }
        public DateTime AdoptionDate { get; set; }

        public Animal MapToAnimal(MyDatabase db)
        {
            return new Animal
            {
                AnimalSpecies = db.AnimalSpecies.FirstOrDefault(a => a.Id == AnimalSpeciesId),
                Name = Name,
                Age = Age,
                AggressionAnimals = db.Aggression.FirstOrDefault(a => a.Id == AggressionAnimalsId),
                AggressionHumans = db.Aggression.FirstOrDefault(a => a.Id == AggressionHumansId),
                Note = Note,
                Breed = db.Breed.FirstOrDefault(a => a.Id == BreedId),
                WeightKg = WeightKg,
                Size = db.Size.FirstOrDefault(a => a.Id == SizeId),
                NeedMedication = NeedMedication,
                Origin = db.Origin.FirstOrDefault(a => a.Id == OriginId),
                Status = db.Status.FirstOrDefault(a => a.Id == StatusId),
                ArriveDate = ArriveDate,
                AdoptionDate = AdoptionDate
            };
        }
    }
}
