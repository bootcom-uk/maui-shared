using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Extensions
{
    public static class HttpClientExtensions
    {

        internal static readonly string _refreshTokenUrl = "https://auth.bootcom.co.uk/login/refresh";

        public async static Task<T> FromJsonStream<T>(this Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead)
            {
                throw new NotSupportedException("Can't read from this stream.");
            }

            return await JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(this HttpRequestMessage req)
        {
            HttpRequestMessage clone = new(req.Method, req.RequestUri);

            // Copy the request's content (via a MemoryStream) into the cloned object
            var ms = new MemoryStream();
            if (req.Content != null)
            {
                await req.Content.CopyToAsync(ms).ConfigureAwait(false);
                ms.Position = 0;
                clone.Content = new StreamContent(ms);

                // Copy the content headers
                if (req.Content.Headers != null)
                    foreach (var h in req.Content.Headers)
                        clone.Content.Headers.Add(h.Key, h.Value);
            }

            clone.Version = req.Version;

            foreach (KeyValuePair<string, IEnumerable<string>> header in req.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return clone;
        }

    }
}
