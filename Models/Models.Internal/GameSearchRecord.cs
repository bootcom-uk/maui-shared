using CommunityToolkit.Mvvm.ComponentModel;

namespace Models.Internal
{
    public partial class GameSearchRecord : ObservableObject
    {

        [ObservableProperty]
        int id;

        [ObservableProperty]
        string name;

        [ObservableProperty]
        string coverUrl;

        [ObservableProperty]
        string url;

        [ObservableProperty]
        string summary;

    }
}
