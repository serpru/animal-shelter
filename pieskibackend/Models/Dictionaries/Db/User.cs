using pieskibackend.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pieskibackend.Models.Dictionaries.Db
{
    public class User : BaseEntity
    {
        [Required]
        [Column("LOGIN")]
        public string Login { get; set; }
        [Required]
        [Column("PASSWORD")]
        public string Password { get; set; }
        [Required]
        [ForeignKey("ROLE_ID")]
        public UserRole Role { get; set; }
        [Column("ACCESS_TOKEN")]
        public string? AccessToken { get; set; }

    }
}
