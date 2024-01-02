using Microsoft.EntityFrameworkCore;
using pieskibackend.Models;
using pieskibackend.Models.Dictionaries.Db;
using System.Text.Json.Serialization;
using TodoApi.Models;

namespace pieskibackend.Api.Requests
{
    public class VisitAddRequest
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
        [JsonPropertyName("adoption_id")]
        public int AdoptionEventId { get; set; }
        [JsonPropertyName("note")]
        public string? Note { get; set; }

        public ResponseWrapper<Visit> MapToVisit(MyDatabase db)
        {
            var adoptionEvent = db.AdoptionEvent
                .Include( x => x.AdoptionStatus )
                .FirstOrDefault(x => x.Id == AdoptionEventId);
            string? errorText = null;

            if (adoptionEvent.AdoptionStatus.Id == (int)Enums.AdoptionStatus.Fail) 
            {
                errorText = "Cannot add visit to finished adoption event.";
            }
            if (adoptionEvent.AdoptionStatus.Id == (int)Enums.AdoptionStatus.Adopted) 
            {
                errorText = "Cannot add visit to finished adoption event.";
            }
            if (adoptionEvent.AdoptionStatus.Id == (int)Enums.AdoptionStatus.Verifying) 
            {
                errorText = "Cannot add visit before adoptee's personal data has been verified.";
            }

            if (errorText != null)
            {
                return new ResponseWrapper<Visit>()
                {
                    Status = Enums.ResponseStatus.Error,
                    Message = errorText,
                    Data = null
                };
            }

            var test = Date.ToUniversalTime();

            var visit = new Visit(
                Date,
                adoptionEvent.Id,
                db.VisitStatus.FirstOrDefault(x => x.Id == (int)Enums.VisitStatusEnum.Upcoming),
                Note
                );

            return new ResponseWrapper<Visit>()
            {
                Status = Enums.ResponseStatus.Success,
                Message = "Added visit to database.",
                Data = visit
            };
        }
    }
}
