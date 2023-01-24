using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Identity.Client;

namespace WpfWAM.UI
{

    public partial class App : Application
    {
        
        private const string GraphApiEndpoint = "https://graph.microsoft.com/v1.0/me";

        //Set the scope for API call to user.read
        private readonly string[] _scopes = { "user.read" };
        private IPublicClientApplication _clientApp;

        private const string ClientId = "0222de5a-9d82-4c97-bf62-62ffc6465aca";
        private const string TenantId = "common"; // 67722499-5640-479f-ba3d-81eda8de611e

        
    }
    
    
}
