using Interfaces;

namespace Services
{
    public class DateTimeService : IDateTimeService
    {

        public IRestService RestService { get; }

        public DateTimeService(IRestService restService)
        {
            RestService = restService;
        }

        public async Task<DateTime?> CurrentDateTime()
        {
            var httpRequest = RestService.CreateRequestMessage(new Uri("https://common.bootcom.co.uk/DateTime"), System.Net.Http.HttpMethod.Get);
            var httpResponse = await RestService.MakeRequest<DateTime>(httpRequest.RequestMessage, httpRequest.TokenSource.Token, null);

            if (!httpResponse.Success)
            {
                return null;
            }

            return httpResponse.Result;
        }

        private readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public Task<DateTime> FromEpochDateTime(long ticks)
        {
            return Task.FromResult<DateTime>(epoch.AddSeconds(ticks));
        }

    }
}
