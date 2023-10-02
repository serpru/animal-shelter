using pieskibackend.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace pieskibackend.Models.Dictionaries.Db;

[Table("JOB_TYPE")]
public class JobType : BaseEntity
{
    public string Description { get; set; }
}

