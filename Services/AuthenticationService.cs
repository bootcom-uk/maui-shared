using Interfaces;
using Models.Internal;
using System.Net.Http.Headers;

namespace Services
{
    public class AuthenticationService : IAuthenticationService<Guid>
    {

        public readonly IRestService RestService;

        public AuthenticationService(IRestService restService)
        {
            RestService = restService;
        }

        public async Task<Guid> CollectUserId()
        {
            
            if(Guid.TryParse(Settings.UserId.ToString(), out var userId))
            {
                return userId;
            }

            var requestMessage = RestService.CreateRequestMessage(new Uri("https://common.bootcom.co.uk/user"), HttpMethod.Get);
            var response = await RestService.MakeRequest<Guid?>(requestMessage.RequestMessage, requestMessage.TokenSource.Token, null);
            if (!response.Success)
            {
                return default;
            }
            return response.Result.Value;
        }

        public async Task<bool> RequestLogin(string emailAddress, string uriScheme)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Content-Type", "application/x-www-form-urlencoded" }
            };
            var requestMessage = RestService.CreateRequestMessage(new Uri("https://auth.bootcom.co.uk/login/RequestLogin"), System.Net.Http.HttpMethod.Post, headers);

            var nameValueCollection = new List<KeyValuePair<string, string>>()
            {
                { new KeyValuePair<string, string>("EmailAddress", emailAddress) },
                    {new KeyValuePair<string, string>("UriScheme", uriScheme) },
                    {new KeyValuePair<string, string>("DeviceId", Settings.DeviceId?.ToString()) },

            };

            requestMessage.RequestMessage.Content = new FormUrlEncodedContent(nameValueCollection);

            var httpResponse = await RestService.MakeRequest(requestMessage.RequestMessage, requestMessage.TokenSource.Token, null);
            return httpResponse.Success;
        }

        public async Task<string> ValidateEmailToken(Guid emailToken)
        {
            var requestMessage = RestService.CreateRequestMessage(new Uri($"https://auth.bootcom.co.uk/AccessCode?code={emailToken}"), HttpMethod.Get);
            var headers = new Dictionary<string, string>()
                {
                    { "deviceId", Settings.DeviceId?.ToString() }
                };
            var responseMessage = await RestService.MakeRequest<string>(requestMessage.RequestMessage, requestMessage.TokenSource.Token, headers);

            if (responseMessage.Success)
            {
                return responseMessage.Result;
            }

            return null;
        }

        public async Task<Dictionary<string, string>> ValidateQuickAccessCode(string quickAccessCode)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Content-Type", "application/x-www-form-urlencoded" }
            };

            var requestMessage = RestService.CreateRequestMessage(new Uri($"https://auth.bootcom.co.uk/Login/CompleteLogin/QuickAccess/{quickAccessCode}"), HttpMethod.Post);


            var nameValueCollection = new List<KeyValuePair<string, string>>()
            {
                    {new KeyValuePair<string, string>("DeviceId", Settings.DeviceId?.ToString()) }
            };

            requestMessage.RequestMessage.Content = new FormUrlEncodedContent(nameValueCollection);

            var httpResponse = await RestService.MakeRequest<Dictionary<string, string>>(requestMessage.RequestMessage, requestMessage.TokenSource.Token, null);

            if (!httpResponse.Success)
            {
                return null;
            }

            Settings.UserToken = httpResponse.Result["token"];
            Settings.RefreshToken = httpResponse.Result["refreshToken"];

            Settings.UserId = await CollectUserId();

            RestService.WebToken = Settings.UserToken;

            return httpResponse.Result;

        }
    }
}
