using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Media.Animation;
using oecu_GoogleDriveShareChecker.Datas;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using oecu_GoogleDriveShareChecker.Windows;
using System.Windows;
using System.Windows.Automation.Provider;

namespace oecu_GoogleDriveShareChecker.Services;

public class CoreService : ICoreService
{
    private const string RegistryBasePath = @"Software\oecu\GoogleDriveShareChecker\ServiceAccounts";
    private const string JsonValueName = "json";

    private readonly IWindowProvider _windowProvider;

    
    public CoreService(IWindowProvider windowProvider)
    {
        _windowProvider = windowProvider;
    }


    //サービスアカウント
    public static void SaveCredentialToRegistry(string serviceAccountEmail, string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
            throw new FileNotFoundException("JSON ファイルが見つかりません。", jsonFilePath);

        string json = File.ReadAllText(jsonFilePath);
        string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

        using var baseKey = Registry.CurrentUser.CreateSubKey(Path.Combine(RegistryBasePath, serviceAccountEmail));
        baseKey?.SetValue(JsonValueName, encoded, RegistryValueKind.String);
    }
    public static ObservableCollection<ServiceAccountData>? ListRegisteredServiceAccounts()
    {
        using var rootKey = Registry.CurrentUser.CreateSubKey(RegistryBasePath, writable: false);
        ObservableCollection<ServiceAccountData> list = new ObservableCollection<ServiceAccountData>();

        if (rootKey == null)
        {
            return list;
        }

        var keys = rootKey?.GetValueNames();

        if (keys == null || keys.Length == 0)
        {
            return list;
        }


        foreach (var name in keys)
        {
            var json = rootKey.GetValue(name) as string;
            if (!string.IsNullOrWhiteSpace(json))
            {
                list.Add(new ServiceAccountData
                {
                    ServiceAccountEmail = name,
                    Json = json
                });
            }
        }
        return list;
    }

    public static string? LoadCredentialsFromRegistry(string serviceAccountEmail)
    {
        using var baseKey = Registry.CurrentUser.OpenSubKey(Path.Combine(RegistryBasePath, serviceAccountEmail));
        var encoded = baseKey?.GetValue(JsonValueName) as string;
        if (string.IsNullOrEmpty(encoded)) return null;

        return Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
    }
    public static void DeleteCredential(string serviceAccountEmail)
    {
        using var rootKey = Registry.CurrentUser.OpenSubKey(RegistryBasePath, writable: true);
        rootKey?.DeleteSubKey(serviceAccountEmail, throwOnMissingSubKey: false);
    }

    public static void OpenServiceAccount(string ServiceAccountEmail, string Json)
    {
        var dialog = new ServiceAccountEditor();

        dialog.model.ServiceAccountEmail = ServiceAccountEmail;
        dialog.model.Json = Json;

        dialog.Owner = Application.Current.MainWindow; // モーダルにするために Owner を指定
        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            // 保存された場合の処理
        }

    }
}