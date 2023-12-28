using Models.Internal;

namespace Interfaces
{
    public interface IRestService
    {

        Uri BaseAddress { get; set; }

        string? WebToken { get; set; }

        HttpRequest CreateRequestMessage<MessageBodyType>(Uri uri, HttpMethod method, MessageBodyType bodyType);

        HttpRequest CreateRequestMessage(Uri uri, HttpMethod method);

        Task<HttpResponse<RequestedResponseType>> MakeRequest<RequestedResponseType>(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken, Dictionary<string, string> headers);

        Task<HttpResponse> MakeRequest(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken, Dictionary<string, string> headers);


    }
}
