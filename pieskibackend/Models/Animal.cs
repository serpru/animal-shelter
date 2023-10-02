using pieskibackend.Models.Dictionaries.Db;
using pieskibackend.Models.Dictionaries.Shorts;
using pieskibackend.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace pieskibackend.Models;

public class Animal : BaseEntity
{
    [Required]
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [Required]
    [Column("BIRTH_DATE")]
    [JsonPropertyName("birth_date")]
    public DateTime BirthDate { get; set; }
    [Required]
    [ForeignKey("AGGRESSION_ANIMALS_ID")]
    [JsonPropertyName("aggression_animals")]
    public Aggression AggressionAnimals { get; set; }
    [Required]
    [ForeignKey("AGGRESSION_HUMANS_ID")]
    [JsonPropertyName("aggression_humans")]
    public Aggression AggressionHumans { get; set; }
    public string Note { get; set; }
    [Required]
    [ForeignKey("BREED_ID")]
    [JsonPropertyName("breed")]
    public Breed Breed { get; set; }
    [Column("WEIGHT_KG")]
    [JsonPropertyName("weight_kg")]
    public double WeightKg { get; set; }
    [Column("NEED_MEDICATION")]
    [JsonPropertyName("need_medication")]
    public bool NeedMedication { get; set; }
    [Required]
    [ForeignKey("ORIGIN_ID")]
    [JsonPropertyName("origin")]
    public Origin Origin { get; set; }
    [Required]
    [ForeignKey("STATUS_ID")]
    [JsonPropertyName("status")]
    public Status Status { get; set; }
    [Required]
    [Column("ARRIVE_DATE")]
    [JsonPropertyName("arrive_date")]
    public DateTime ArriveDate { get; set; }
    [Column("ADOPTION_DATE")]
    [JsonPropertyName("adoption_date")]
    public DateTime? AdoptionDate { get; set;}

    public Animal() { }
    public Animal(string name, DateTime birthDate, Aggression aggressionAnimals, Aggression aggressionHumans, string note, Breed breed, double weightKg, bool needMedication, Origin origin, Status status, DateTime arriveDate, DateTime? adoptionDate)
    {
        Name = name;
        BirthDate = birthDate;
        AggressionAnimals = aggressionAnimals;
        AggressionHumans = aggressionHumans;
        Note = note;
        Breed = breed;
        WeightKg = weightKg;
        NeedMedication = needMedication;
        Origin = origin;
        Status = status;
        ArriveDate = arriveDate;
        AdoptionDate = adoptionDate;
    }

    public AnimalShort ToAnimalShort()
    {
        return new AnimalShort(this.Id, this.Name, this.Breed, this.BirthDate);
    }
}
