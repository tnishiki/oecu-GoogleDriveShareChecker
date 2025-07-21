using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oecu_GoogleDriveShareChecker.ViewModels
{
    public partial class TreeViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _Name = string.Empty;

        [ObservableProperty]
        private ObservableCollection<TreeViewModel> _Children = new ObservableCollection<TreeViewModel>();

        public TreeViewModel(string _name)
        {
            _Name = _name;
        }
    }
}
