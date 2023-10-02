using Microsoft.EntityFrameworkCore;
using pieskibackend.Models;
using pieskibackend.Models.Dictionaries;
using TodoApi.Models;

namespace pieskibackend.Api.Requests
{
    public class AnimalEditRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? AggressionAnimalsId { get; set; }
        public int? AggressionHumansId { get; set; }
        public string? Note { get; set; }
        public int? BreedId { get; set; }
        public int? WeightKg { get; set; }
        public int? SizeId { get; set; }
        public bool? NeedMedication { get; set; }
        public int? OriginId { get; set; }
        public int? StatusId { get; set; }
        public DateTime? ArriveDate { get; set; } = null;
        public DateTime? AdoptionDate { get; set; }

        public ResponseWrapper<Animal> MapToAnimal(MyDatabase db)
        {
            var animalDb = db.Animal
                .Include(x => x.AggressionAnimals)
                .Include(x => x.AggressionHumans)
                .Include(x => x.Breed)
                .Include(x => x.Origin)
                .Include(x => x.Status)
                .FirstOrDefault(x => x.Id == Id);
            if (animalDb == null)
            {
                return new ResponseWrapper<Animal>()
                {
                    Status = Enums.ResponseStatus.Error,
                    Message = "No animal with this ID exists in the database.",
                    Data = null
                };
            }

            var breedDb = db.Breed
                .Include(x => x.AnimalSpecies)
                .FirstOrDefault(x => x.Id == BreedId);

            if (Name != null) { animalDb.Name = Name; }
            if (BirthDate != null) { animalDb.BirthDate = (DateTime)BirthDate; }
            if (AggressionAnimalsId!= null) { animalDb.AggressionAnimals = db.Aggression.FirstOrDefault(a => a.Id == AggressionAnimalsId); }
            if (AggressionHumansId!= null) { animalDb.AggressionHumans = db.Aggression.FirstOrDefault(a => a.Id == AggressionHumansId); }
            if (Note != null) { animalDb.Note = Note; }
            if (BreedId !=null) { animalDb.Breed = db.Breed.FirstOrDefault(a => a.Id == BreedId); }
            if (WeightKg!= null) { animalDb.WeightKg = (int)WeightKg; }     
            if (NeedMedication !=null) { animalDb.NeedMedication = (bool)NeedMedication; }
            if (OriginId != null) { animalDb.Origin = db.Origin.FirstOrDefault(a => a.Id == OriginId); }
            if (StatusId!= null) { animalDb.Status = db.Status.FirstOrDefault(a => a.Id == StatusId); }
            if (ArriveDate != null) { animalDb.ArriveDate = (DateTime)ArriveDate;}
            if (AdoptionDate != null) { animalDb.AdoptionDate = AdoptionDate; }
            

            return new ResponseWrapper<Animal>()
            {
                Status = Enums.ResponseStatus.Success,
                Message = "Animal successfully updated.",
                Data = animalDb
            };
        }
    }
}
