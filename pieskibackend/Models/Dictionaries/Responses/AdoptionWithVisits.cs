using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using pieskibackend.Models.Shared;
using pieskibackend.Models.Dictionaries.Db;
using pieskibackend.Models.Dictionaries.Shorts;

namespace pieskibackend.Models.Dictionaries.Responses
{
    public class AdoptionWithVisits : BaseEntity
    {
        [JsonPropertyName("adoptee")]
        public Adoptee Adoptee { get; set; }
        [JsonPropertyName("employee")]
        public EmployeeShort Employee { get; set; }
        [JsonPropertyName("adoption_status")]
        public AdoptionStatus AdoptionStatus { get; set; }
        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }
        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }
        [JsonPropertyName("animal")]
        public AnimalShort Animal { get; set; }
        [JsonPropertyName("note")]
        public string Note { get; set; }
        [JsonPropertyName("visits")]
        public List<Visit> Visits { get; set; }

        //public AdoptionWithVisits() { }

        public AdoptionWithVisits(AdoptionEvent adoption, List<Visit> visits)
        {
            Id = adoption.Id;
            Adoptee = adoption.Adoptee;
            Employee = adoption.Employee.ToEmployeeShort();
            AdoptionStatus = adoption.AdoptionStatus;
            StartDate = adoption.StartDate;
            EndDate = adoption.EndDate;
            Animal = adoption.Animal.ToAnimalShort();
            Note = adoption.Note;
            Visits = visits;
        }

        public AdoptionWithVisits(Adoptee adoptee, Employee employee, AdoptionStatus adoptionStatus, DateTime startDate, DateTime? endDate, Animal animal, string note, List<Visit> visits)
        {
            Adoptee = adoptee;
            Employee = employee.ToEmployeeShort();
            AdoptionStatus = adoptionStatus;
            StartDate = startDate;
            EndDate = endDate;
            Animal = animal.ToAnimalShort();
            Note = note;
            Visits = visits;
        }
    }
}
