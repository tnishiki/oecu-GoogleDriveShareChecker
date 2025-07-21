using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace oecu_GoogleDriveShareChecker.Datas
{
    public partial class ServiceAccountData: ObservableObject
    {
        [ObservableProperty]
        private string _ServiceAccountEmail = string.Empty;

        [ObservableProperty]
        private string _Json = string.Empty;
    }
}
