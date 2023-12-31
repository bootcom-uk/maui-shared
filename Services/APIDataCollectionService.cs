using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class APIDataCollectionService : IAPIDataCollectionService
    {
        public Task<IEnumerable<ReturnType>> CollectData<ReturnType>(Uri collectionUri, string dateTimeFieldName, string lastModifiedHeader = "LastModified")
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<KeyType>> CollectDeletedRecords<KeyType>(Uri collectionUri, string dateTimeFieldName)
        {
            throw new NotImplementedException();
        }
    }
}
