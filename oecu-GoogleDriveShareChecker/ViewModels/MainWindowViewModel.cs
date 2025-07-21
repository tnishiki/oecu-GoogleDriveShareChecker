using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using oecu_GoogleDriveShareChecker.Datas;
using oecu_GoogleDriveShareChecker.Services;

namespace oecu_GoogleDriveShareChecker.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ServiceAccountData>? _ServiceAccountList = null;
    [ObservableProperty]
    private ServiceAccountData? _SelectedAccount = null;

    [ObservableProperty]
    private ObservableCollection<TreeViewModel> _TreeItems = new ObservableCollection<TreeViewModel>
        {
            new TreeViewModel("Root1")
            {
                Children = {
                    new TreeViewModel("Child1"),
                    new TreeViewModel("Child2")
                }
            },
            new TreeViewModel("Root2")
        };

    public MainWindowViewModel()
    {
        LoadServiceAccountList();
    }
    public void LoadServiceAccountList()
    {
        ServiceAccountList = CoreService.ListRegisteredServiceAccounts();
    }
}
