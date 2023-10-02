using pieskibackend.Models.Dictionaries;
using pieskibackend.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TodoApi.Models;
using Microsoft.EntityFrameworkCore;
using pieskibackend.Api.Enums;
using System.Text.Json.Serialization;

namespace pieskibackend.Api.Requests
{
    public class AdoptionEventEditRequest
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("employee_id")]
        public int EmployeeId { get; set; }
        [JsonPropertyName("adoption_status_id")]
        public int AdoptionStatusId { get; set; }
        [JsonPropertyName("note")]
        public string? Note { get; set; } = null;

        public ResponseWrapper<AdoptionEvent> MapToAdoptionEvent(MyDatabase db)
        {
            var adoption = db.AdoptionEvent
                .Include(x => x.Employee)
                .Include(x => x.Adoptee)
                .Include(x => x.Animal)
                .Include(x => x.AdoptionStatus)
                .FirstOrDefault(x => x.Id == Id);

            if (adoption == null)
            {
                return new ResponseWrapper<AdoptionEvent>()
                {
                    Status = Enums.ResponseStatus.Error,
                    Message = "No adoption with this ID exists in the database.",
                    Data = null
                };
            }

            adoption.Employee = db.Employee.FirstOrDefault(x => x.Id == EmployeeId);
            adoption.AdoptionStatus = db.AdoptionStatus.FirstOrDefault(x => x.Id == AdoptionStatusId);
            if (AdoptionStatusId == (int)Enums.AdoptionStatus.Adopted)
            {
                adoption.EndDate = DateTime.Now;
                adoption.Animal.AdoptionDate = DateTime.Now;
                adoption.Animal.Status = db.Status.FirstOrDefault(x => x.Id == (int)Enums.AnimalStatus.Adopted);
            } else if (AdoptionStatusId == (int)Enums.AdoptionStatus.Fail)
            {
                adoption.EndDate = DateTime.Now;
                adoption.Animal.Status = db.Status.FirstOrDefault(x => x.Id == (int)Enums.AnimalStatus.Available);
            }
            adoption.Note = Note;


            return new ResponseWrapper<AdoptionEvent>()
            {
                Status = Enums.ResponseStatus.Success,
                Message = "Updated adoption info in the database.",
                Data = adoption
            };
        }
    }
}
