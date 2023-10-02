using Microsoft.EntityFrameworkCore;
using pieskibackend.Models.Dictionaries.Shorts;

namespace pieskibackend.Models.Dictionaries.Responses
{
    [Keyless]
    public class AnimalResponse
    {
        public Animal Animal { get; set; }
        public List<AdoptionShort>? Adoptions { get; set; }

        public AnimalResponse(Animal animal, List<AdoptionShort> adoptions) 
        { 
            Animal = animal;
            Adoptions = adoptions;
        }
    }
}
