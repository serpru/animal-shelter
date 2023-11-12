using Microsoft.EntityFrameworkCore;
using pieskibackend.Models;
using System.Text.Json.Serialization;
using TodoApi.Models;

namespace pieskibackend.Api.Requests
{
    public class EmployeeEditRequest
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }
        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }
        [JsonPropertyName("phone_number")]
        public string? PhoneNumber { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("salary")]
        public double? Salary { get; set; }
        [JsonPropertyName("job_type_id")]
        public int? JobTypeId { get; set; }
        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }
        [JsonPropertyName("birth_date")]
        public DateTime? BirthDate { get; set; }
        public ResponseWrapper<Employee> MapToEmployee(MyDatabase db)
        {
            var employee = db.Employee
                .Include(x => x.JobType)
                .FirstOrDefault(x => x.Id == Id);

            if (employee == null)
            {
                return new ResponseWrapper<Employee>()
                {
                    Status = Enums.ResponseStatus.Error,
                    Message = "No employee with this ID exists in the database.",
                    Data = null
                };
            }

            if (FirstName != null) { employee.FirstName = FirstName; }
            if (LastName != null) { employee.LastName = LastName; }
            if (PhoneNumber != null) { employee.PhoneNumber = PhoneNumber; }
            if (Email != null) { employee.Email = Email; }
            //if (Salary != null) { employee.Salary = (double)Salary; }
            if (JobTypeId != null) { employee.JobType = db.JobType.FirstOrDefault(a => a.Id == JobTypeId); }
            if (StartDate != null) { employee.StartDate = (DateTime)StartDate; }
            //if (BirthDate != null) { employee.BirthDate = (DateTime)BirthDate; }


            return new ResponseWrapper<Employee>()
            {
                Status = Enums.ResponseStatus.Success,
                Message = "Edited employee in the database.",
                Data = employee
            };
        }
    }
}
