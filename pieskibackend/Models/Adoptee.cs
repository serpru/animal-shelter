using pieskibackend.Models.Dictionaries;
using pieskibackend.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace pieskibackend.Models
{
    public class Adoptee : BaseEntity
    {
        [Required]
        [Column("FIRST_NAME")]
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        [Required]
        [Column("LAST_NAME")]
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
        [Required]
        [Column("PHONE_NUMBER")]
        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }
        [Required]
        [Column("EMAIL")]
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [Required]
        [Column("ADDRESS")]
        [JsonPropertyName("address")]
        public string Address { get; set; }
        [Required]
        [Column("CITY")]
        [JsonPropertyName("city")]
        public string City { get; set; }
        [Required]
        [Column("ZIPCODE")]
        [JsonPropertyName("zip_code")]
        public string Zipcode { get; set; }
        [Required]
        [Column("COUNTRY")]
        [JsonPropertyName("country")]
        public string Country { get; set; }

        public Adoptee() { }
        public Adoptee(string firstName, string lastName, string phoneNumber, string email, string address, string city, string zipcode, string country)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Email = email;
            Address = address;
            City = city;
            Zipcode = zipcode;
            Country = country;
        }
    }
}
