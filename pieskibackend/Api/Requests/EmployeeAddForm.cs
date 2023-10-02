using Microsoft.EntityFrameworkCore;
using pieskibackend.Models.Dictionaries.Db;

namespace pieskibackend.Api.Requests
{
    [Keyless]
    public class EmployeeAddForm
    {
        public List<JobType> JobTypes { get; set; }

        public EmployeeAddForm() { }

        public EmployeeAddForm(
            List<JobType> jobTypes)
        {
            JobTypes = jobTypes;
        }
    }
}
