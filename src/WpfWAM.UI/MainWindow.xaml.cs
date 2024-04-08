using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Graph;
using Microsoft.Identity.Client;

using Prompt = Microsoft.Identity.Client.Prompt;

namespace WpfWAM.UI;

public partial class MainWindow
{
    private readonly IPublicClientApplication _client;
    private readonly string[] _scopes;
    private readonly GraphServiceClient _graphApi;

    public MainWindow(IPublicClientApplication client, string[] scopes)
    {
        _client = client;
        _scopes = scopes;
        InitializeComponent();
        SignoutButton.Visibility = Visibility.Collapsed;
    }

    protected override async void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);

        await SignIn();
    }

    private async Task SignIn()
    {
        AuthenticationResult? authResult;
        var accounts = await _client.GetAccountsAsync();
        var firstAccount = accounts.FirstOrDefault();

        try
        {
            authResult = await _client.AcquireTokenSilent(_scopes, firstAccount).ExecuteAsync();
        }
        catch (MsalUiRequiredException ex)
        {
            System.Diagnostics.Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

            authResult = await AcquireTokenInteractive();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error Acquiring Token Silently:{Environment.NewLine}{ex}");
            return;
        }

        if (authResult != null)
        {
            SignoutButton.Visibility = Visibility.Visible;

            // Using Microsoft.Identity.Client to get the user's profile
            var api = new GraphApi(authResult.AccessToken);
            var me = await api.GetMe();

            // Parse the permissions from the token
            var permissions = api.GetPermissions();

            System.Diagnostics.Debug.WriteLine($"Graph api: me: {Environment.NewLine}{me}");
            System.Diagnostics.Debug.WriteLine($"Permissions: {Environment.NewLine}{string.Join(',', permissions)}");
         }
    }

    private async Task<AuthenticationResult?> AcquireTokenInteractive()
    {
        try
        {
            var accounts = await _client.GetAccountsAsync();

            var result = await _client.AcquireTokenInteractive(_scopes)
                .WithAccount(accounts.FirstOrDefault())
                .WithParentActivityOrWindow(new WindowInteropHelper(this).Handle)
                .WithPrompt(Prompt.NoPrompt)
                .ExecuteAsync();

            return result;
        }
        catch (MsalException msalEx)
        {
            System.Diagnostics.Debug.WriteLine($"Error Acquiring Token:{Environment.NewLine}{msalEx}");
        }

        return null;
    }

    public async Task<AuthenticationResult> AcquireTokenSilent()
    {
        var accounts = await _client.GetAccountsAsync();
        var result = await _client.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
            .ExecuteAsync();
        return result;
    }

    private async Task<bool>SignOut()
    {
        try
        {
            var accounts = await _client.GetAccountsAsync();
            var account = accounts.FirstOrDefault();
            if(account == null) return false;

                try
                {
                    await _client.RemoveAsync(account);
                    System.Diagnostics.Debug.WriteLine("User has signed-out");
                    return true;
                }
                catch (MsalException ex)
                {

                    System.Diagnostics.Debug.WriteLine($"Error signing-out user: {ex.Message}");
                }
        }
        catch (Exception)
        {
            // Ignore
        }
        return false;
    }

    private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var signedOut = await SignOut();
        if(signedOut)
            SignoutButton.Visibility = Visibility.Collapsed;
    }
}
