using CommunityToolkit.Mvvm.ComponentModel;

namespace Models.Internal
{
    public partial class Setting : ObservableObject
    {
        [ObservableProperty]
        int id;

        [ObservableProperty]
        string settingName;

        [ObservableProperty]
        string settingValue;

    }
}
