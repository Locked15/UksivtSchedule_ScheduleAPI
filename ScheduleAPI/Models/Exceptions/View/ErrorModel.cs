using System.Net;

namespace ScheduleAPI.Models.Exceptions.View
{
    public class ErrorModel
    {
        public Guid RequestID { get; set; }

        public HttpStatusCode ErrorCode { get; set; }

        public string? Message { get; set; }
    }
}
