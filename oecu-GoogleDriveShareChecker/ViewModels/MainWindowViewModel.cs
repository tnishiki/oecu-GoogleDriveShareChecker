using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using oecu_GoogleDriveShareChecker.Datas;
using oecu_GoogleDriveShareChecker.Services;
using oecu_GoogleDriveShareChecker.Windows;

namespace oecu_GoogleDriveShareChecker.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ServiceAccountData>? _ServiceAccountList = null;
    [ObservableProperty]
    private ServiceAccountData? _SelectedAccount = null;

    [ObservableProperty]
    private ObservableCollection<TreeViewModel> _OrganizationTree = new ObservableCollection<TreeViewModel>
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

    [ObservableProperty]
    private ObservableCollection<ShareDriveData> _ShareDriveList = new ObservableCollection<ShareDriveData>();
    [ObservableProperty]
    private ShareDriveData? _SelectedShareDrive = null;


    public MainWindowViewModel()
    {
        LoadServiceAccountList();

        ShareDriveList.Add(new ShareDriveData
        {
            Name = "sharedrive"
        });
    }
    public void LoadServiceAccountList()
    {
        ServiceAccountList = CoreService.ListRegisteredServiceAccounts();
    }
    [RelayCommand]
    public void CreateNewServiceAccount()
    {
        CoreService.OpenServiceAccount("", "");
    }
    [RelayCommand]
    public void ModifyServiceAccount()
    {
        if (SelectedAccount == null)
        {
            return;
        }
        CoreService.OpenServiceAccount(SelectedAccount.ServiceAccountEmail, SelectedAccount.Json);

        ServiceAccountList = CoreService.ListRegisteredServiceAccounts();
    }

    public void ReloadOrgTree(ServiceAccountData sacount)
    {
        //sacount;
    }
}
