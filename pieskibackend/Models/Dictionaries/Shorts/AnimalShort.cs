using pieskibackend.Models.Dictionaries.Db;
using pieskibackend.Models.Shared;
using System.Text.Json.Serialization;

namespace pieskibackend.Models.Dictionaries.Shorts
{
    public class AnimalShort: BaseEntity
    {
        [JsonPropertyName("name")]
        public string AnimalName { get; set; }
        [JsonPropertyName("breed")]
        public Breed Breed { get; set; }
        [JsonPropertyName("birth_date")]
        public DateTime BirthDate { get; set; }

        public AnimalShort(int id, string name, Breed breed, DateTime birthDate)
        {
            Id = id;
            AnimalName = name;
            Breed = breed;
            BirthDate = birthDate;
        }
    }
}
