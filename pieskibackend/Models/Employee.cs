using pieskibackend.Models.Dictionaries.Db;
using pieskibackend.Models.Dictionaries.Shorts;
using pieskibackend.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace pieskibackend.Models
{
    public class Employee : BaseEntity
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
        //[Required]
        //[Column("SALARY")]
        //[JsonPropertyName("salary")]
        //public double Salary { get; set; }
        [Required]
        [ForeignKey("JOB_TYPE_ID")]
        [JsonPropertyName("job_type")]
        public JobType JobType { get; set; }
        [Required]
        [Column("START_DATE")]
        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }
        //[Required]
        //[Column("BIRTH_DATE")]
        //[JsonPropertyName("birth_date")]
        //public DateTime BirthDate { get; set; }

        public Employee() { }

        public Employee(string firstName, string lastName, string phoneNumber, string email, double salary, JobType jobType, DateTime startDate, DateTime birthDate)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Email = email;
            //Salary = salary;
            JobType = jobType;
            StartDate = startDate;
            //BirthDate = birthDate;
        }

        public EmployeeShort ToEmployeeShort()
        {
            return new EmployeeShort(this.Id, this.FirstName, this.LastName);
        }
    }
}
