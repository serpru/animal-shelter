using pieskibackend.Models.Shared;
using System.Text.Json.Serialization;

namespace pieskibackend.Models.Dictionaries.Shorts
{
    public class EmployeeShort: BaseEntity
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        public EmployeeShort(int id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
