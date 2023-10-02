using pieskibackend.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace pieskibackend.Models.Dictionaries.Db
{
    [Table("VISIT")]
    public class Visit : BaseEntity
    {
        [Required]
        [Column("DATE")]
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
        [Required]
        [Column("ADOPTION_EVENT_ID")]
        [JsonPropertyName("adoption_id")]
        public int AdoptionEventId { get; set; }
        [Required]
        [ForeignKey("VISIT_STATUS_ID")]
        [JsonPropertyName("visit_status")]
        public VisitStatus VisitStatus { get; set; }

        public Visit() { }
        public Visit(DateTime date, int adoptionEventId, VisitStatus visitStatus)
        {
            Date = date;
            AdoptionEventId = adoptionEventId;
            VisitStatus = visitStatus;
        }
    }
}
