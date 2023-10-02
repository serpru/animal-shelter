using pieskibackend.Api.Enums;
using pieskibackend.Models;

namespace pieskibackend.Api.Requests
{
    public class ResponseWrapper<T>
    {
        public ResponseStatus Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
