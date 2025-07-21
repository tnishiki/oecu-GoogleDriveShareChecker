using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace oecu_GoogleDriveShareChecker.Services
{
    public interface IWindowProvider
    {
        Window? GetMainWindow();
    }
}
