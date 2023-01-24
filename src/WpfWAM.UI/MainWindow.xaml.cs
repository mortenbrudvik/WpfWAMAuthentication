using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Interop;
using Microsoft.Identity.Client;

namespace WpfWAM.UI;

public partial class MainWindow
{
    private readonly IPublicClientApplication _client;
    private readonly string[] _scopes;

    public MainWindow(IPublicClientApplication client, string[] scopes)
    {
        _client = client;
        _scopes = scopes;
        InitializeComponent();
    }

    protected override async void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
            
        await Signin();
    }

    private async Task Signin()
    {
        AuthenticationResult? authResult = null;
        var accounts = await _client.GetAccountsAsync();
        var firstAccount = accounts.FirstOrDefault();

        try
        {
            authResult = await _client.AcquireTokenSilent(_scopes, firstAccount).ExecuteAsync();
        }
        catch (MsalUiRequiredException ex)
        {
            System.Diagnostics.Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");
            try
            {
                authResult = await _client.AcquireTokenInteractive(_scopes)
                    .WithAccount(firstAccount)
                    .WithParentActivityOrWindow(new WindowInteropHelper(this).Handle) // optional, used to center the browser on the window
                    .WithPrompt(Prompt.NoPrompt)
                    .ExecuteAsync();
            }
            catch (MsalException msalEx)
            {
                System.Diagnostics.Debug.WriteLine($"Error Acquiring Token:{Environment.NewLine}{msalEx}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error Acquiring Token Silently:{Environment.NewLine}{ex}");
            return; 
        }
            
        if (authResult != null)
        {
            var api = new GraphApi(authResult.AccessToken);
            var me = await api.GetMe();
            
            System.Diagnostics.Debug.WriteLine($"Token content: {Environment.NewLine}{me}");
        }
    }

    private async Task<bool> SignOut()
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
    
}