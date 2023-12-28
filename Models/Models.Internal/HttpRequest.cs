namespace Models.Internal
{
    public class HttpRequest
    {

        public HttpRequestMessage? RequestMessage { get; set; }

        public CancellationTokenSource? TokenSource { get; set; }

    }
}
