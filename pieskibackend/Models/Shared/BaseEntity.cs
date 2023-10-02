using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace pieskibackend.Models.Shared;

public class BaseEntity
{
    [Required]
    [JsonPropertyName("id")]
    public int Id { get; set; }
}
