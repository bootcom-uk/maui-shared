using CommunityToolkit.Mvvm.ComponentModel;
using MongoDB.Bson;

namespace SanityTests.Models
{
    public partial class CollectionMapping : ObservableObject
    {

        [ObservableProperty]
        string collectionName;

        [ObservableProperty]
        string databaseName;

        [ObservableProperty]
        List<string> fields;

        [ObservableProperty]
        int version;

        [ObservableProperty]
        ObjectId lastId;

    }
}
