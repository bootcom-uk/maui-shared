namespace Services
{
    public class HttpService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public HttpRequestBuilder CreateBuilder(Uri url, HttpMethod method)
        {
            return HttpRequestBuilder.Create(_httpClientFactory)
                .WithUrl(url)
                .WithMethod(method);
        }
    }
}
