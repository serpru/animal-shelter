using pieskibackend.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace pieskibackend.Models.Dictionaries.Db
{
    [Table("VISIT_STATUS")]
    public class VisitStatus : BaseEntity
    {
        [Required]
        [Column("DESCRIPTION")]
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
