using pieskibackend.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace pieskibackend.Models.Dictionaries.Db
{
    [Table("ADOPTION_STATUS")]
    public class AdoptionStatus : BaseEntity
    {
        [Required]
        [Column("DESCRIPTION")]
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
