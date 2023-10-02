using pieskibackend.Models.Dictionaries;
using pieskibackend.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TodoApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace pieskibackend.Api.Requests
{
    public class AdoptionEventAddRequest
    {
        [JsonPropertyName("adoptee_id")]
        public int AdopteeId { get; set; }
        [JsonPropertyName("employee_id")]
        public int EmployeeId { get; set; }
        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }
        [JsonPropertyName("animal_id")]
        public int AnimalId { get; set; }
        [JsonPropertyName("note")]
        public string? Note { get; set; } = null;

        public ResponseWrapper<AdoptionEvent> MapToAdoptionEvent(MyDatabase db)
        {
            var existingAdoption = db.AdoptionEvent
                .Include(x => x.AdoptionStatus)
                .Include(x => x.Animal)
                .FirstOrDefault(x => x.Animal.Id == AnimalId);

            if (existingAdoption != null)
            {
                if ((existingAdoption.AdoptionStatus.Id != (int)Enums.AdoptionStatus.Adopted)
                    || (existingAdoption.AdoptionStatus.Id != (int)Enums.AdoptionStatus.Fail))
                {
                    return new ResponseWrapper<AdoptionEvent>()
                    {
                        Status = Enums.ResponseStatus.Error,
                        Message = "Ongoing adoption already exists for this animal.",
                        Data = null
                    };
                }
            }
            if (StartDate == null)
            {
                StartDate = DateTime.Now;
            }

            var adoptee = db.Adoptee.FirstOrDefault(x => x.Id == AdopteeId);
            var employee = db.Employee.FirstOrDefault(x => x.Id == EmployeeId);
            var adoptionStatus = db.AdoptionStatus.FirstOrDefault(x => x.Id == (int)Enums.AdoptionStatus.Start);
            var animal = db.Animal.FirstOrDefault(x => x.Id == AnimalId);

            var adoption = new AdoptionEvent(
                adoptee,
                employee,
                adoptionStatus,
                (DateTime)StartDate,
                null,
                animal,
                Note
                );

            animal.Status = db.Status.FirstOrDefault(x => x.Id == (int)Enums.AnimalStatus.InProgress);

            return new ResponseWrapper<AdoptionEvent>()
            {
                Status = Enums.ResponseStatus.Success,
                Message = "Added adoption event to database.",
                Data = adoption
            };
        }
    }
}
