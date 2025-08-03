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
    private ObservableCollection<AccountData> _AccountList;

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

        AccountList = new ObservableCollection<AccountData>();

        AccountList.Add(new AccountData() { Account = "nishiki@osakac.ac.jp", UserName = "NISHIKI Takeshi", });
        AccountList.Add(new AccountData() { Account = "asa-k@osakac.ac.jp", UserName = "ASAJIMA Kota", });
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
    [RelayCommand]
    public void ReloadOrgTree()
    {
        if (SelectedAccount == null)
        {
            return;
        }

        //組織ツリーを取得

        var a = CoreService.GetOrgList(SelectedAccount);


    }
}
