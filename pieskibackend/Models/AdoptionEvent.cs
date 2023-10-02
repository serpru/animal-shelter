using pieskibackend.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using pieskibackend.Models.Dictionaries.Db;

namespace pieskibackend.Models
{
    [Table("ADOPTION_EVENT")]
    public class AdoptionEvent : BaseEntity
    {
        [Required]
        [ForeignKey("ADOPTEE_ID")]
        [JsonPropertyName("adoptee")]
        public Adoptee Adoptee { get; set; }
        [Required]
        [ForeignKey("EMPLOYEE_ID")]
        [JsonPropertyName("employee")]
        public Employee Employee { get; set; }
        [Required]
        [ForeignKey("ADOPTION_STATUS_ID")]
        [JsonPropertyName("adoption_status")]
        public AdoptionStatus AdoptionStatus { get; set; }
        [Required]
        [Column("START_DATE")]
        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }
        [Column("END_DATE")]
        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }
        [Required]
        [ForeignKey("ANIMAL_ID")]
        [JsonPropertyName("animal")]
        public Animal Animal { get; set; }
        [Column("NOTE")]
        [JsonPropertyName("note")]
        public string? Note { get; set; }

        public AdoptionEvent() { }

        public AdoptionEvent(Adoptee adoptee, Employee employee, AdoptionStatus adoptionStatus, DateTime startDate, DateTime? endDate, Animal animal, string note)
        {
            Adoptee = adoptee;
            Employee = employee;
            AdoptionStatus = adoptionStatus;
            StartDate = startDate;
            EndDate = endDate;
            Animal = animal;
            Note = note;
        }
    }
}
