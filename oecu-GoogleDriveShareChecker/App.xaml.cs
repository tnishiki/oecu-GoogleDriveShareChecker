using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Runtime.InteropServices.JavaScript;
using System.Windows;
using oecu_GoogleDriveShareChecker.Services;

namespace oecu_GoogleDriveShareChecker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // ViewModels
                    services.AddSingleton<MainWindow>();

                    // Views
                    services.AddSingleton<MainWindow>();

                    // Services（必要に応じて）
                    services.AddSingleton<ICoreService, CoreService>();
                })
                .Build();

            _host.Start();

            // MainWindow 起動
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            _host?.Dispose();
            base.OnExit(e);
        }
    }
}