using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;

namespace WpfWAM.UI;

public static class ClientApp
{
    // Below are the clientId (Application Id) of your app registration and the tenant information.
    // You have to replace:
    // - the content of ClientID with the Application Id for your app registration
    // - The content of Tenant by the information about the accounts allowed to sign-in in your application:
    //   - For Work or School account in your org, use your tenant ID, or domain
    //   - for any Work or School accounts, use organizations
    //   - for any Work or School accounts, or Microsoft personal account, use common
    //   - for Microsoft Personal account, use consumers
    private static string Instance = "https://login.microsoftonline.com/";

    public static IPublicClientApplication Create(string clientId, string tenantId)
    {
        var brokerOptions = new BrokerOptions(BrokerOptions.OperatingSystems.Windows);

        var clientApp = PublicClientApplicationBuilder.Create(clientId)
            .WithAuthority($"{Instance}{tenantId}")
            .WithDefaultRedirectUri()
            .WithBroker(brokerOptions)
            .Build();

        var cacheHelper = CreateCacheHelperAsync().GetAwaiter().GetResult();

        // Let the cache helper handle MSAL's cache, otherwise the user will be prompted to sign-in every time.
        cacheHelper.RegisterCache(clientApp.UserTokenCache);

        return clientApp;
    }

    private static async Task<MsalCacheHelper> CreateCacheHelperAsync()
    {
        // Since this is a WPF application, only Windows storage is configured
        var storageProperties = new StorageCreationPropertiesBuilder(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".msalcache.bin",
                MsalCacheHelper.UserRootDirectory)
            .Build();

        var cacheHelper = await MsalCacheHelper.CreateAsync(
                storageProperties,
                new TraceSource("MSAL.CacheTrace"))
            .ConfigureAwait(false);

        return cacheHelper;
    }
}
