using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace pieskibackend.Api.Requests
{
    public class LoginRequest
    {
        [Required]
        [Column("LOGIN")]
        [JsonPropertyName("login")]
        public string Login { get; set; }
        [Required]
        [Column("PASSWORD")]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
