namespace Interfaces
{
    public interface IRemoteFileStorage
    {

        public Task Remove(IRemoteFileRequest fileRequest);

        public Task<Stream> Get(IRemoteFileRequest fileRequest); 

        public Task<string> Set(IRemoteFileRequest fileRequest, Stream stream);

    }
}
