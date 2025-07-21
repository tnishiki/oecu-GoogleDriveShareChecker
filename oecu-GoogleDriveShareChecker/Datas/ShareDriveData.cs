using CommunityToolkit.Mvvm.ComponentModel;

namespace oecu_GoogleDriveShareChecker.Datas
{
    public partial class ShareDriveData: ObservableObject
    {
        [ObservableProperty]
        private string _Name = string.Empty;
        [ObservableProperty]
        private string _Url = string.Empty;
    }
}
