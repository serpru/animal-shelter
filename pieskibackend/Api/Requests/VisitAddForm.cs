using Microsoft.EntityFrameworkCore;
using pieskibackend.Models.Dictionaries.Db;
using pieskibackend.Models.Dictionaries.Shorts;
using System.Text.Json.Serialization;

namespace pieskibackend.Api.Requests
{
    [Keyless]
    public class VisitAddForm
    {
        [JsonPropertyName("adoptions")]
        public List<AdoptionShort> Adoptions { get; set; }
        [JsonPropertyName("visit_statuses")]
        public List<VisitStatus> VisitStatuses { get; set; }

        public VisitAddForm() { }

        public VisitAddForm(List<AdoptionShort> adoptions, List<VisitStatus> visitStatuses)
        {
            Adoptions = adoptions;
            VisitStatuses = visitStatuses;
        }
    }
}
