using System.Net;

namespace MyServices.API.Services
{
    public class HttpClientServiceResponse<T>
    {
        public T Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ResponseContent { get; set; }
    }
}
