using pieskibackend.Models.Shared;

namespace pieskibackend.Models
{
    public class User : BaseEntity
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
