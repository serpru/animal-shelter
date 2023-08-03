using pieskibackend.Models.Dictionaries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pieskibackend.Models;

public class Animal
{
    [Required]
    public int Id { get; set; }
    [Required]
    [ForeignKey("ANIMAL_SPECIES_ID")]
    public AnimalSpecies AnimalSpecies { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public int Age { get; set; }
    [Required]
    [ForeignKey("AGGRESSION_ANIMALS_ID")]
    public Aggression AggressionAnimals { get; set; }
    [Required]
    [ForeignKey("AGGRESSION_HUMANS_ID")]
    public Aggression AggressionHumans { get; set; }
    public string Note { get; set; }
    [Required]
    [ForeignKey("BREED_ID")]
    public Breed Breed { get; set; }
    [Column("WEIGHT_KG")]
    public int WeightKg { get; set; }
    [Required]
    [ForeignKey("SIZE_ID")]
    public Size Size { get; set; }
    [Column("NEED_MEDICATION")]
    public bool NeedMedication { get; set; }
    [Required]
    [ForeignKey("ORIGIN_ID")]
    public Origin Origin { get; set; }
    [Required]
    [ForeignKey("STATUS_ID")]
    public Status Status { get; set; }
    [Required]
    [Column("ARRIVE_DATE")]
    public DateTime ArriveDate { get; set; }
    [Column("ADOPTION_DATE")]
    public DateTime? AdoptionDate { get; set;}
}
