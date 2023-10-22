using pieskibackend.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace pieskibackend.Models.Dictionaries.Db
{
    [Table("USER_ROLE")]
    public class UserRole: BaseEntity
    {
        [Required]
        [Column("DESCRIPTION")]
        public string Description { get; set; }
    }
}
