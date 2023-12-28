namespace Models.Internal
{
    public class HttpResponse
    {
        public bool Success { get; set; }

        public string? Exception { get; set; }
    }

    public class HttpResponse<ResponseType> : HttpResponse
    {
        public ResponseType? Result { get; set; }
    }
}
