using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WpfWAM.UI;

public class GraphApi
{
    private readonly string _graphUrl;
    private readonly string _accessToken;

    public GraphApi(string accessToken)
    {
        _graphUrl = "https://graph.microsoft.com/v1.0";
        _accessToken = accessToken;
    }
    
    public async Task<string> GetMe()
    {
        var httpClient = new HttpClient();
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _graphUrl + "/me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var response = await httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
}