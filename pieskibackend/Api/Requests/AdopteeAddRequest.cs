using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using pieskibackend.Models.Dictionaries;
using pieskibackend.Models;
using TodoApi.Models;

namespace pieskibackend.Api.Requests
{
    public class AdopteeAddRequest
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("address")]
        public string Address { get; set; }
        [JsonPropertyName("city")]
        public string City { get; set; }
        [JsonPropertyName("zipcode")]
        public string Zipcode { get; set; }
        [JsonPropertyName("country")]
        public string Country { get; set; }

        public ResponseWrapper<Adoptee> MapToAdoptee(MyDatabase db)
        {
            var adoptee = new Adoptee(FirstName, LastName, PhoneNumber, Email, Address, City, Zipcode, Country);

            var existingAdoptees = db.Adoptee.Where(x => x.LastName == adoptee.LastName).ToList();

            if (existingAdoptees != null)
            {
                foreach (Adoptee person in existingAdoptees)
                {
                    if (adoptee.FirstName == person.FirstName)
                    {
                        return new ResponseWrapper<Adoptee>()
                        {
                            Status = Enums.ResponseStatus.Error,
                            Message = "Adoptee already exists.",
                            Data = null
                        };
                    }
                }
            }

            return new ResponseWrapper<Adoptee>()
            {
                Status = Enums.ResponseStatus.Success,
                Message = "Added adoptee to database.",
                Data = adoptee
            };
        }
    }
}
