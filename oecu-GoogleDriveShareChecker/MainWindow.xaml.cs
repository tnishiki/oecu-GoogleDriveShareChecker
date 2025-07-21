using oecu_GoogleDriveShareChecker.Services;
using oecu_GoogleDriveShareChecker.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace oecu_GoogleDriveShareChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel model = new MainWindowViewModel();

        public MainWindow(ICoreService coreService)
        {
            InitializeComponent();

            DataContext = model;

        }
    }
}