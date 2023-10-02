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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }

        public ResponseWrapper<Adoptee> MapToAdoptee(MyDatabase db)
        {
            var adoptee = new Adoptee(FirstName, LastName, PhoneNumber, Email, Address, City, Zipcode, Country);


            return new ResponseWrapper<Adoptee>()
            {
                Status = Enums.ResponseStatus.Success,
                Message = "Added adoptee to database.",
                Data = adoptee
            };
        }
    }
}
