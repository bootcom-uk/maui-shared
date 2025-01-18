using Interfaces;
using Models.Internal;
using System.Net;
using System.Text.Json;

namespace Services
{
    public class HttpService : IHttpService
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public HttpService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public HttpRequestMessage GenerateRequestMessage(Uri url, HttpMethod method, Dictionary<string, string>? headers = null)
        {
            var httpRequestMessage = new HttpRequestMessage()
            {
                RequestUri = url,
                Method = method
            };

            if(headers != null)
            {
                foreach (var  header in headers)
                {
                    httpRequestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            return httpRequestMessage;
        }

        public async Task<HttpResponse<T?>> SendAsync<T>(HttpRequestMessage request)
        {

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var responseMessage = await httpClient.SendAsync(request);

                return new HttpResponse<T?>()
                {
                    StatusCode = responseMessage.StatusCode,
                    Success = true,
                    Result = JsonSerializer.Deserialize<T>(await responseMessage.Content.ReadAsStreamAsync())
                };
                 

            } catch(WebException wex)
            {
                return new HttpResponse<T?>()
                {
                    Success = false,
                    Exception = wex.StackTrace
                };
            }
            
            
        }
    }
}
