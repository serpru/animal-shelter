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
        [ForeignKey("ROLE_ID")]
        public UserRole Role { get; set; }
        [Column("ACCESS_TOKEN")]
        public string? AccessToken { get; set; }
        [Column("PASSWORD_HASH")]
        public byte[] PasswordHash { get; set; }
        [Column("PASSWORD_SALT")]
        public byte[] PasswordSalt { get; set; }

    }
}
