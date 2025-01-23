using System.Net;

namespace Models.Internal
{
    // Generic response class
    public class HttpResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool Success { get; set; }
        public T? Result { get; set; }
        public string? Exception { get; set; }
    }

    // Non-generic response class
    public class HttpResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool Success { get; set; }
        public string? Result { get; set; } // Optional response body as string
        public string? Exception { get; set; }
    }
}
