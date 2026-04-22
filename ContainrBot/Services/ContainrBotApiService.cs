using ContainrBot.Services;

namespace ContainrBot.Services;

public class ContainrBotApiService(
    IConfiguration configuration,
    HttpClient httpClient) : IContainrBotApiService
{
    public string ListContainers(out bool success)
    {
        var baseUrl = configuration.GetValue<string>("CONTAINRBOTAPI_BASEURL");
        var url = $"{baseUrl}/list";

        httpClient.Timeout = TimeSpan.FromSeconds(60);

        HttpResponseMessage response = httpClient.GetAsync(url).Result;

        success = response.IsSuccessStatusCode;

        return response.Content.ReadAsStringAsync().Result;
    }

    public string StartContainers(string name, out bool success)
    {
        var baseUrl = configuration.GetValue<string>("CONTAINRBOTAPI_BASEURL");
        var url = $"{baseUrl}/start/{name}";

        httpClient.Timeout = TimeSpan.FromSeconds(60);

        HttpResponseMessage response = httpClient.GetAsync(url).Result;

        success = response.IsSuccessStatusCode;

        return response.Content.ReadAsStringAsync().Result;
    }

    public string StopContainers(string name, out bool success)
    {
        var baseUrl = configuration.GetValue<string>("CONTAINRBOTAPI_BASEURL");
        var url = $"{baseUrl}/stop/{name}";

        httpClient.Timeout = TimeSpan.FromSeconds(60);

        HttpResponseMessage response = httpClient.GetAsync(url).Result;

        success = response.IsSuccessStatusCode;

        return response.Content.ReadAsStringAsync().Result;
    }
}
