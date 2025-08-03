using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Text.Json;
using System.Windows;
using System.Windows.Automation.Provider;
using System.Xml.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Admin.Directory.directory_v1.Data;
using oecu_GoogleDriveShareChecker.Datas;
using oecu_GoogleDriveShareChecker.Windows;
using static System.Net.WebRequestMethods;

namespace oecu_GoogleDriveShareChecker.Services;

public class CoreService : ICoreService
{
    private const string RegistryBasePath = @"Software\oecu\GoogleDriveShareChecker\ServiceAccounts";
    private const string JsonValueName = "json";


    private readonly IWindowProvider _windowProvider;

    public string connectionString = string.Empty;


    public CoreService(IWindowProvider windowProvider)
    {
        _windowProvider = windowProvider;

        if (!CheckLocalDB())
        {

        }
    }

    public bool CheckLocalDB()
    {
        bool result = false;

        try
        {
            string folderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "OECU",
                "oecu-GoogleDriveShareChecker");

            Directory.CreateDirectory(folderPath);
            string dbPath = Path.Combine(folderPath, "local.db");

            connectionString = $"Data Source={dbPath}";

            if (System.IO.File.Exists(dbPath))
            {
                result = true;
                return result;
            }
            else
            {
                // データベースとテーブルの作成
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    string createTableSql = @"
                        CREATE TABLE IF NOT EXISTS Users (
                          Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL,CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP)";

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = createTableSql;
                        command.ExecuteNonQuery();
                    }

                    Console.WriteLine("データベースとテーブルを作成しました。");

                    connectionString = $"Data Source={dbPath}";
                    result = true;

                }
            }
        }
        catch
        {
        }
        return result;
    }


    public async Task<DirectoryService> GetGoogleDirectoryService(string json)
    {
        string[] scopes = { DirectoryService.Scope.AdminDirectoryOrgunitReadonly };
        var credential = GoogleCredential.FromJson(json).CreateScoped(scopes);


        DirectoryService service = new DirectoryService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "OrgUnit Reader"
        });

        var request = service.Orgunits.List("my_customer"); // または顧客ID
        request.Type = OrgunitsResource.ListRequest.TypeEnum.All;
        request.OrgUnitPath = "/";

        var response = await request.ExecuteAsync();

        foreach (var orgUnit in response.OrganizationUnits)
        {
            Console.WriteLine($"{orgUnit.Name} ({orgUnit.OrgUnitPath})");
        }

        return service;
    }

    //
    //サービスアカウントをレジストリに保存
    //
    public static void SaveCredentialToRegistry(string serviceAccountEmail, string json)
    {
        string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

        using var baseKey = Registry.CurrentUser.CreateSubKey(RegistryBasePath);
        baseKey?.SetValue(serviceAccountEmail, encoded, RegistryValueKind.String);
    }
    //
    // 指定のサービスアカウントをレジストリから削除する
    //
    public static void DeleteCredentialFromRegistry(string serviceAccountEmail)
    {
        using var baseKey = Registry.CurrentUser.OpenSubKey(RegistryBasePath, writable: true);
        if (baseKey == null)
        {
            // 該当するキーが存在しない場合は何もしない
            return;
        }

        try
        {
            baseKey.DeleteValue(serviceAccountEmail, throwOnMissingValue: false);
        }
        catch (Exception ex)
        {
            // 必要に応じてログ出力や例外処理
            Console.WriteLine($"レジストリの削除に失敗しました: {ex.Message}");
        }
    }

    public static ObservableCollection<ServiceAccountData>? ListRegisteredServiceAccounts()
    {
        using var rootKey = Registry.CurrentUser.CreateSubKey(RegistryBasePath, writable: false);
        List<ServiceAccountData> list = new List<ServiceAccountData>();

        if (rootKey == null)
        {
            return null;
        }

        var keys = rootKey?.GetValueNames();

        if (keys == null || keys.Length == 0)
        {
            return null;
        }


        foreach (var name in keys)
        {
            var json = rootKey!.GetValue(name) as string;
            if (!string.IsNullOrWhiteSpace(json))
            {
                list.Add(new ServiceAccountData
                {
                    ServiceAccountEmail = name,
                    Json = Encoding.UTF8.GetString(Convert.FromBase64String(json))
                });
            }
        }
        list.Sort((a, b) =>
        {
            return string.Compare(a.ServiceAccountEmail, b.ServiceAccountEmail);
        });

        ObservableCollection<ServiceAccountData> outlist = new ObservableCollection<ServiceAccountData>();

        foreach (var service in list)
        {
            outlist.Add(service);
        }

        return outlist;
    }

    //サービスアカウントのJSONキーをレジストリから取得
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

        dialog.model.SetServiceAccount(ServiceAccountEmail, Json);

        dialog.Owner = Application.Current.MainWindow; // モーダルにするために Owner を指定
        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            // 保存された場合の処理
            var (newaccount, newjson) = dialog.model.GetServiceAccount();

            //まずは元のキーを削除
            DeleteCredentialFromRegistry(ServiceAccountEmail);

            //新しいキーを保存
            SaveCredentialToRegistry(newaccount, newjson);
        }
    }
    //
    //サービスアカウントの内容チェック
    //
    public static bool CheckServiceAccountKey(string key)
    {
        try
        {
            using var doc = JsonDocument.Parse(key);
            var root = doc.RootElement;

            // 必須キーの存在チェック
            string[] requiredKeys = new[]
            {
                "type", "project_id", "private_key_id", "private_key", "client_email",
                "client_id", "auth_uri", "token_uri", "auth_provider_x509_cert_url",
                "client_x509_cert_url", "universe_domain"
            };

            foreach (var keyName in requiredKeys)
            {
                if (!root.TryGetProperty(keyName, out var _))
                    return false;
            }

            // 詳細バリデーション
            if (root.GetProperty("type").GetString() != "service_account")
                return false;

            if (!IsEmailValid(root.GetProperty("client_email").GetString()))
                return false;

            if (!IsClientIdValid(root.GetProperty("client_id").GetString()))
                return false;

            if (!IsPrivateKeyValid(root.GetProperty("private_key").GetString()))
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsEmailValid(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return email.EndsWith(".iam.gserviceaccount.com", StringComparison.OrdinalIgnoreCase)
            && email.Contains("@");
    }

    private static bool IsClientIdValid(string? id)
    {
        return !string.IsNullOrWhiteSpace(id);
    }

    private static bool IsPrivateKeyValid(string? key)
    {
        return !string.IsNullOrWhiteSpace(key)
            && key.Contains("-----BEGIN PRIVATE KEY-----")
            && key.Contains("-----END PRIVATE KEY-----");
    }


    public static List<OrgUnit>? GetOrgList(ServiceAccountData serviceAccountData)
    {
        var service = CreateDirectoryService(serviceAccountData);

        // 組織ツリー（すべての OrgUnit を階層構造で）を取得
        return ListOrgUnitTree(service);
    }
    //
    //ディレクトリサービスを取得する
    //
    private static DirectoryService CreateDirectoryService(ServiceAccountData serviceAccountData)
    {
        // 2) Admin SDK の読み取り専用スコープ
        string[] scopes = { DirectoryService.Scope.AdminDirectoryOrgunitReadonly };

        // 3) ドメイン管理者ユーザー（ドメインワイド・デリゲーションで許可済み）のメールアドレス
        const string adminUser = "mc2adm@osakac.ac.jp";

        GoogleCredential credential = GoogleCredential
            .FromJson(serviceAccountData.Json)
            .CreateScoped(scopes)            // スコープをセット
            .CreateWithUser(adminUser);      // 管理者ユーザーを委任

        // 4) Directory API クライアントを初期化
        return new DirectoryService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "oecu-GoogleDriveShareChecker",

        });
    }
    /// <summary>
    /// 組織ユニットをツリー形式で列挙してコンソールに出力
    /// </summary>
    private static List<OrgUnit>? ListOrgUnitTree(DirectoryService service)
    {
        try
        {
            var request = service.Orgunits.List("my_customer");
            request.Type = OrgunitsResource.ListRequest.TypeEnum.All;

            OrgUnits response = request.Execute();

            if (response == null)
            {
                return null;
            }
            else
            {
                return (List<OrgUnit>)response!.OrganizationUnits;
            }
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}