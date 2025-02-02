
using CommunityToolkit.Mvvm.ComponentModel;
using MongoDB.Bson;

namespace SanityTests.Models
{
    public partial class AppSyncMapping : ObservableObject
    {

        [ObservableProperty]
        ObjectId id;

        [ObservableProperty]
        int version;

        [ObservableProperty]
        int fullRefreshIfNoActivityInDays;

        [ObservableProperty]
        List<CollectionMapping> collections;

        [ObservableProperty]
        string appName;
    }
}
