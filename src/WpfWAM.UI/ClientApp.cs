using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Broker;

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

    public static IPublicClientApplication Create(string clientId, string tenantId, bool useWam = true)
    {
        var builder = PublicClientApplicationBuilder.Create(clientId)
            .WithAuthority($"{Instance}{tenantId}")
            .WithDefaultRedirectUri();

        //Use of Broker Requires redirect URI "ms-appx-web://microsoft.aad.brokerplugin/{client_id}" in app registration
        if (useWam) builder.WithBrokerPreview();
        var clientApp = builder.Build();
        TokenCacheHelper.EnableSerialization(clientApp.UserTokenCache);

        return clientApp;
    }
}