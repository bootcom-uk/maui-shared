using Models.Internal;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Services
{
    public class HttpRequestBuilder
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpRequestMessage _requestMessage;
        private int _retryCount = 0;
        private Func<HttpResponseMessage, bool>? _retryCondition;
        private readonly Dictionary<HttpStatusCode, Func<Task>> _statusHandlers = new();
        private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        // Private constructor to enforce usage of the static Create method
        private HttpRequestBuilder(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _requestMessage = new HttpRequestMessage();
        }

        // Static factory method
        public static HttpRequestBuilder Create(IHttpClientFactory httpClientFactory)
        {
            return new HttpRequestBuilder(httpClientFactory);
        }

        // Set the request URL
        public HttpRequestBuilder WithUrl(Uri url)
        {
            _requestMessage.RequestUri = url;
            return this;
        }

        // Set the HTTP method
        public HttpRequestBuilder WithMethod(HttpMethod method)
        {
            _requestMessage.Method = method;
            return this;
        }

        // Add a header to the request
        public HttpRequestBuilder WithHeader(string key, string value)
        {
            _requestMessage.Headers.Add(key, value);
            return this;
        }

        // Add JSON content to the request
        public HttpRequestBuilder WithJsonContent<T>(T content)
        {
            var json = JsonSerializer.Serialize(content);
            _requestMessage.Content = new StringContent(json);
            _requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return this;
        }

        // Add form data to the request
        public HttpRequestBuilder WithFormContent(Dictionary<string, string> formData)
        {            
            var content = new MultipartFormDataContent();
            foreach(var item in formData)
            {
                content.Add(new StringContent(item.Value, Encoding.UTF8, MediaTypeNames.Text.Plain), item.Key);
            }
            
            _requestMessage.Content = content;

            return this;
        }

        // Configure retry logic
        public HttpRequestBuilder WithRetry(int retryCount, Func<HttpResponseMessage, bool>? retryCondition = null)
        {
            _retryCount = retryCount;
            _retryCondition = retryCondition;
            return this;
        }

        // Define an action for a specific HTTP status code
        public HttpRequestBuilder OnStatus(HttpStatusCode statusCode, Func<Task> action)
        {
            _statusHandlers[statusCode] = action;
            return this;
        }

        // Ensure the request is properly configured
        private void ValidateRequest()
        {
            if (_requestMessage.RequestUri == null)
                throw new InvalidOperationException("Request URL must be set.");
            if (_requestMessage.Method == null)
                throw new InvalidOperationException("HTTP method must be set.");
        }

        // Send the request and handle retries/status codes
        public async Task<HttpResponse<T?>> SendAsync<T>()
        {
            ValidateRequest();

            int attempts = 0;
            var httpClient = _httpClientFactory.CreateClient();

            while (true)
            {
                try
                {
                    var response = await httpClient.SendAsync(_requestMessage);

                    // Handle specific status codes
                    if (_statusHandlers.TryGetValue(response.StatusCode, out var handler))
                    {
                        await handler(); // Perform the custom action
                        continue;        // Retry the original request
                    }

                    // Retry based on a condition
                    if (_retryCondition != null && attempts < _retryCount && _retryCondition(response))
                    {
                        attempts++;
                        continue;
                    }

                    // Return the response
                    return new HttpResponse<T?>()
                    {
                        StatusCode = response.StatusCode,
                        Success = response.IsSuccessStatusCode,
                        Result = response.IsSuccessStatusCode
                            ? JsonSerializer.Deserialize<T>(await response.Content.ReadAsStreamAsync(), serializerOptions)
                            : default
                    };
                }
                catch (WebException wex)
                {
                    if (attempts >= _retryCount)
                    {
                        return new HttpResponse<T?>()
                        {
                            Success = false,
                            Exception = wex.ToString()
                        };
                    }

                    attempts++;
                }
                catch (Exception ex)
                {
                    if (attempts >= _retryCount)
                    {
                        return new HttpResponse<T?>()
                        {
                            Success = false,
                            Exception = ex.Message
                        };
                    }

                    attempts++;
                }
            }
        }
    }
}
