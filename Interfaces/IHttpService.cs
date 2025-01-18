using Models.Internal;
using System.Net;

namespace Interfaces
{
    public interface IHttpService
    {

        Task<HttpResponse<T?>> SendAsync<T>(HttpRequestMessage request);

        HttpRequestMessage GenerateRequestMessage(Uri url, HttpMethod method, Dictionary<string, string>? headers = null);

    }
}
