using pieskibackend.Models.Shared;
using System.Text.Json.Serialization;

namespace pieskibackend.Models.Dictionaries.Shorts
{
    public class AdopteeShort: BaseEntity
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        public AdopteeShort(int id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
