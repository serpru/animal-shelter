using pieskibackend.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace pieskibackend.Models.Dictionaries;

[Table("ANIMAL_SPECIES")]
public class AnimalSpecies : BaseEntity
{
    public string Species { get; set; }
}
