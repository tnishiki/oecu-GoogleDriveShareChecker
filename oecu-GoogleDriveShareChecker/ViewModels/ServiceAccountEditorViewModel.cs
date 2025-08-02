using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using oecu_GoogleDriveShareChecker.Services;
using oecu_GoogleDriveShareChecker.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oecu_GoogleDriveShareChecker.ViewModels;

public partial class ServiceAccountEditorViewModel : ObservableObject
{
    public ServiceAccountEditor? EditorWindow = null;


    private string OriginalServiceAccountEmail = "";

    [ObservableProperty]
    private string _ServiceAccountEmail = string.Empty;

    [ObservableProperty]
    private string _Json = string.Empty;

    [ObservableProperty]
    private bool _CanSave = false;


    public void SetServiceAccount(string _account, string _json)
    {
        OriginalServiceAccountEmail = _account;
        ServiceAccountEmail = _account;
        Json = _json;
    }
    public (string _account, string _json) GetServiceAccount()
    {
        return (ServiceAccountEmail, Json);

    }

    // ServiceAccountEmail の変更時に CanSave を通知
    partial void OnServiceAccountEmailChanged(string value)
    {
        OnPropertyChanged(nameof(CanSave));
    }

    // Json の変更時に CanSave を通知
    partial void OnJsonChanged(string value)
    {
        OnPropertyChanged(nameof(CanSave));
    }

    [RelayCommand]
    private void Accept()
    {
        if (EditorWindow != null)
        {
            EditorWindow.DialogResult = true;
            EditorWindow.Close();
        }
    }
    [RelayCommand]
    private void Cancel()
    {
        EditorWindow?.Close();
    }
    [RelayCommand]
    private void CheckKey()
    {
        CanSave = CoreService.CheckServiceAccountKey(Json);
    }
}
