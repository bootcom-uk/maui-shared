namespace Interfaces
{
    public interface IAPIDataCollectionService
    {

        Task<IEnumerable<ReturnType>> CollectData<ReturnType>(Uri collectionUri, string dateTimeFieldName, string lastModifiedHeader = "LastModified");

        Task<IEnumerable<KeyType>> CollectDeletedRecords<KeyType>(Uri collectionUri, string dateTimeFieldName);

    }
}
