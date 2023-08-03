using pieskibackend.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace pieskibackend.Models.Dictionaries;

public class Breed : BaseEntity
{
    public string Name { get; set; }
    [ForeignKey("ANIMAL_SPECIES_ID")]
    public AnimalSpecies AnimalSpecies { get; set; }
}
