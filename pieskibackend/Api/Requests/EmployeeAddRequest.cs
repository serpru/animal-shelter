using pieskibackend.Models;
using System.Text.Json.Serialization;
using TodoApi.Models;

namespace pieskibackend.Api.Requests
{
    public class EmployeeAddRequest
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("salary")]
        public double Salary { get; set; }
        [JsonPropertyName("job_type_id")]
        public int JobTypeId { get; set; }
        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }
        [JsonPropertyName("birth_date")]
        public DateTime BirthDate { get; set; }

        public ResponseWrapper<Employee> MapToEmployee(MyDatabase db)
        {
            var employee = new Employee(
                FirstName, 
                LastName,
                PhoneNumber,
                Email, 
                Salary, 
                db.JobType.FirstOrDefault(a => a.Id == JobTypeId), 
                StartDate, 
                BirthDate);


            return new ResponseWrapper<Employee>()
            {
                Status = Enums.ResponseStatus.Success,
                Message = "Added employee to database.",
                Data = employee
            };
        }
    }
}
