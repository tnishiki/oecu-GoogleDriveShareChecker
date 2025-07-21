using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using oecu_GoogleDriveShareChecker.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oecu_GoogleDriveShareChecker.ViewModels
{
    public partial class ServiceAccountEditorViewModel : ObservableObject
    {
        public ServiceAccountEditor? EditorWindow = null;

        [ObservableProperty]
        private string _ServiceAccountEmail = string.Empty;

        [ObservableProperty]
        private string _Json = string.Empty;


        [RelayCommand]
        private void Accept()
        {
            EditorWindow?.Close();
        }
        [RelayCommand]
        private void Cancel()
        {
            EditorWindow?.Close();
        }
    }
}
