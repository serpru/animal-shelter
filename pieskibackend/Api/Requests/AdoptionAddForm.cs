using pieskibackend.Models;
using pieskibackend.Models.Dictionaries.Db;
using pieskibackend.Models.Dictionaries.Shorts;
using System.Text.Json.Serialization;

namespace pieskibackend.Api.Requests
{
    public class AdoptionAddForm
    {
        [JsonPropertyName("adoptees")]
        public List<AdopteeShort> Adoptions { get; set; }
        [JsonPropertyName("employees")]
        public List<EmployeeShort> Employees { get; set; }
        [JsonPropertyName("statuses")]
        public List<AdoptionStatus> AdoptionStatuses { get; set; }
        [JsonPropertyName("animals")]
        public List<AnimalShort> Animals { get; set; }

        public AdoptionAddForm() { }

        public AdoptionAddForm(List<AdopteeShort> adoptees, List<EmployeeShort> employees, List<AdoptionStatus> adoptionStatuses, List<AnimalShort> animals)
        {
            Adoptions = adoptees;
            Employees = employees;
            AdoptionStatuses = adoptionStatuses;
            Animals = animals;
        }
    }
}
