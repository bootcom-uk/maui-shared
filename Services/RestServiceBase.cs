using Extensions;
using Interfaces;
using Models.Internal;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Services
{
    public abstract class RestServiceBase : IRestService
    {


        public const string AuthURL = "https://bootcomidentity.azurewebsites.net";

        public int ClientTimespanInSeconds { get; set; } = 120;

        /// <summary>
        /// Specifies the base address 
        /// </summary>
        public abstract Uri BaseAddress { get; set; }

        public string? WebToken { get; set; }

        public void CheckToken()
        {
            var token = Settings.UserToken;
            if (string.IsNullOrWhiteSpace(WebToken) && !string.IsNullOrWhiteSpace(token))
            {
                WebToken = token;
            }
        }

        public HttpRequest CreateRequestMessage<MessageBodyType>(Uri uri, HttpMethod method, MessageBodyType bodyType)
        {
            CheckToken();

            var requestMessage = CreateRequestMessage(uri, method);
            var json = JsonSerializer.Serialize(bodyType);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            requestMessage.RequestMessage!.Content = stringContent;
            return requestMessage;
        }

        public HttpRequest CreateRequestMessage(Uri uri, HttpMethod method)
        {
            CheckToken();

            var result = new HttpRequest()
            {
                RequestMessage = new HttpRequestMessage(method, uri),
                TokenSource = new CancellationTokenSource()
            };
            result.RequestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", this.WebToken);
            result.RequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            result.RequestMessage.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            return result;
        }


        public async Task<HttpResponse<RequestedResponseType>> MakeRequest<RequestedResponseType>(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken, Dictionary<string, string> headers)
        {
            var response = new HttpResponse<RequestedResponseType>();

            try
            {

                using var handler = new HttpClientHandler();

#if DEBUG
                handler.ServerCertificateCustomValidationCallback =
                    (message, cert, chain, errors) => { return true; };
#endif

                handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip;

                var httpClient = HttpClientFactory.Create(handler);
                httpClient.Timeout = TimeSpan.FromSeconds(ClientTimespanInSeconds);

                if (headers != null)
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    foreach (var keyValuePair in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(keyValuePair.Key, keyValuePair.Value);
                    }
                }

                var webResponse = await httpClient.SendAsync(httpRequestMessage);

                var responseText = await webResponse.Content.ReadAsStringAsync();

                var tokenExpiredHeaderCollected = webResponse.Headers.WwwAuthenticate.FirstOrDefault();
                var isInValidToken = (tokenExpiredHeaderCollected != null && tokenExpiredHeaderCollected.Parameter!.Contains("invalid_token"));

                webResponse = await UnauthenticatedCheck(webResponse, httpRequestMessage, httpClient, isInValidToken);

                var responseString = await webResponse.Content.ReadAsStringAsync();

                webResponse.EnsureSuccessStatusCode();

                var readResponse = await webResponse.Content.ReadAsStreamAsync();

                response.Result = await readResponse.FromJsonStream<RequestedResponseType>();
                response.Success = true;

            }
            catch (Exception ex)
            {
                response.Exception = ex.ToString();
                response.Success = false;
            }

            return response;
        }

        public async Task<HttpResponse> MakeRequest(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken, Dictionary<string, string> headers)
        {
            var response = new HttpResponse();

            try
            {
                using var handler = new HttpClientHandler();
#if DEBUG
                handler.ServerCertificateCustomValidationCallback =
                    (message, cert, chain, errors) => { return true; };
#endif

                handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip;

                var httpClient = HttpClientFactory.Create(handler);
                httpClient.Timeout = TimeSpan.FromSeconds(ClientTimespanInSeconds);

                if (headers != null)
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    foreach (var keyValuePair in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(keyValuePair.Key, keyValuePair.Value);
                    }
                }

                var webResponse = await httpClient.SendAsync(httpRequestMessage);

                var tokenExpiredHeaderCollected = webResponse.Headers.WwwAuthenticate.FirstOrDefault();
                var isInValidToken = (tokenExpiredHeaderCollected != null && tokenExpiredHeaderCollected.Parameter!.Contains("invalid_token"));

                var responseContent = await webResponse.Content.ReadAsStringAsync();

                webResponse = await UnauthenticatedCheck(webResponse, httpRequestMessage, httpClient, isInValidToken);

                webResponse.EnsureSuccessStatusCode();

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Exception = ex.ToString();
                response.Success = false;
            }

            return response;
        }

        private static async Task<HttpResponseMessage> UnauthenticatedCheck(HttpResponseMessage webResponse, HttpRequestMessage originalRequest, HttpClient httpClient, bool isInValidToken)
        {
            // If the request comes back as unauthorized then we need to to check if 
            // we've got a refresh token. If we do then send this and ideally get ourselves
            // reauthenticated
            if (webResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized && originalRequest.RequestUri!.ToString().ToLower().Contains("bootcom.co.uk") && !string.IsNullOrEmpty(Settings.RefreshToken) && !string.IsNullOrWhiteSpace(Settings.UserToken) && isInValidToken)
            {

                // Create a new request to do a refresh
                var refreshTokenMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"{AuthURL}/RefreshToken"));
                refreshTokenMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", Settings.UserToken);
                refreshTokenMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                refreshTokenMessage.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                refreshTokenMessage.Headers.Add("refreshToken", Settings.RefreshToken);

                var reauthenticateWebResponse = await httpClient.SendAsync(refreshTokenMessage);

                reauthenticateWebResponse.EnsureSuccessStatusCode();

                var reauthenticateOutput = await (await reauthenticateWebResponse.Content.ReadAsStreamAsync()).FromJsonStream<Dictionary<string, string>>();

                // We've reauthenticated into the system so swap over the token
                Settings.UserToken = reauthenticateOutput["token"];
                Settings.RefreshToken = reauthenticateOutput["refreshToken"];


                originalRequest.Headers.Authorization = new AuthenticationHeaderValue("bearer", Settings.UserToken);

                // Resend our original message 
                return await httpClient.SendAsync(await originalRequest.CloneHttpRequestMessageAsync());
            }

            // Return the original response
            return webResponse;

        }

    }
}
