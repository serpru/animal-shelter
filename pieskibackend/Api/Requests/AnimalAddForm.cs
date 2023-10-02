using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using pieskibackend.Models.Dictionaries.Db;

namespace pieskibackend.Api.Requests
{
    [Keyless]
    public class AnimalAddForm
    {
        public List<Aggression> Aggressions { get; set; }
        public List<Breed> Breeds { get; set; }
        public List<Size> Sizes { get; set; }
        public List<Origin> Origins { get; set; }
        public List<Status> Statuses { get; set; }

        public AnimalAddForm() { }

        public AnimalAddForm(
            List<Aggression> aggressions, 
            List<Breed> breeds,
            List<Size> sizes,
            List<Origin> origins,
            List<Status> statuses)
        {
            Aggressions= aggressions;
            Breeds = breeds;
            Sizes = sizes;
            Origins = origins;
            Statuses = statuses;
        }
    }
}
