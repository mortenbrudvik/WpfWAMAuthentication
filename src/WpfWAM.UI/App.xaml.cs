using System.Windows;

namespace WpfWAM.UI;

public partial class App
{
    private readonly string[] _scopes = { "user.read" };

    private const string ClientId = "0222de5a-9d82-4c97-bf62-62ffc6465aca";
    private const string TenantId = "common";

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var clientApp = ClientApp.Create(ClientId, TenantId);

        MainWindow = new MainWindow(clientApp, _scopes);
        MainWindow.Show();
    }
}
