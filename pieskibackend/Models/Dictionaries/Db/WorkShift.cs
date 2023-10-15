using pieskibackend.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace pieskibackend.Models.Dictionaries.Db
{
    [Table("WORKSHIFT")]
    public class WorkShift : BaseEntity
    {
        [Required]
        [ForeignKey("EMPLOYEE_ID")]
        [JsonPropertyName("employee")]
        public Employee Employee { get; set; }
        [Required]
        [Column("SCHEDULE_DATE")]
        [JsonPropertyName("schedule_date")]
        public DateTime ScheduleDate { get; set; }
        [Required]
        [Column("SHIFT_START")]
        [JsonPropertyName("shift_start")]
        public TimeSpan ShiftStart { get; set; }
        [Required]
        [Column("SHIFT_END")]
        [JsonPropertyName("shift_end")]
        public TimeSpan ShiftEnd { get; set; }
    }
}
