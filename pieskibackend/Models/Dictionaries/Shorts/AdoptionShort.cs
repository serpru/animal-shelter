



using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using pieskibackend.Models.Shared;
using System.Text.Json.Serialization;
using pieskibackend.Models.Dictionaries.Db;

namespace pieskibackend.Models.Dictionaries.Shorts
{
    public class AdoptionShort : BaseEntity
    {
        [JsonPropertyName("adoptee_name")]
        public string AdopteeName { get; set; }
        [JsonPropertyName("status")]
        public AdoptionStatus AdoptionStatus { get; set; }
        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }
        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }
        [JsonPropertyName("animal_name")]
        public string AnimalName { get; set; }
        [JsonPropertyName("note")]
        public string Note { get; set; }

        public AdoptionShort(int id, string adoptee, AdoptionStatus adoptionStatus, DateTime startDate, DateTime? endDate, string animalName, string note)
        {
            Id = id;
            AdopteeName = adoptee;
            AdoptionStatus = adoptionStatus;
            StartDate = startDate;
            EndDate = endDate;
            AnimalName = animalName;
            Note = note;
        }
    }
}
