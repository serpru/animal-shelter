using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace pieskibackend.Api.Requests
{
    public class RegisterRequest
    {
        [Required]
        [Column("LOGIN")]
        [JsonPropertyName("login")]
        public string Login { get; set; }
        [Required]
        [Column("PASSWORD")]
        [JsonPropertyName("password")]
        public string Password { get; set; }
        [Required]
        [Column("ROLE_ID")]
        [JsonPropertyName("role_id")]
        public int RoleID { get; set; }
    }
}
