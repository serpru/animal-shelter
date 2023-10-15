using Microsoft.EntityFrameworkCore;
using pieskibackend.Models;
using pieskibackend.Models.Dictionaries.Db;
using System.Text.Json.Serialization;
using TodoApi.Models;

namespace pieskibackend.Api.Requests
{
    public class VisitEditRequest
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
        [JsonPropertyName("visit_status_id")]
        public int VisitStatusId { get; set; }
        [JsonPropertyName("note")]
        public string? Note { get; set; }

        public ResponseWrapper<Visit> MapToVisit(MyDatabase db)
        {
            var existingVisit = db.Visit
                .Include(x => x.VisitStatus)
                .FirstOrDefault(x => x.Id == Id);

            if (existingVisit == null)
            {
                return new ResponseWrapper<Visit>()
                {
                    Status = Enums.ResponseStatus.Error,
                    Message = "No visit with this ID exists in the database.",
                    Data = null
                };
            }

            existingVisit.Date = Date;
            existingVisit.VisitStatus = db.VisitStatus.FirstOrDefault(x => x.Id == VisitStatusId);
            existingVisit.Note = Note;
            return new ResponseWrapper<Visit>()
            {
                Status = Enums.ResponseStatus.Success,
                Message = "Updated the visit in the database.",
                Data = existingVisit
            };
        }
    }
}
