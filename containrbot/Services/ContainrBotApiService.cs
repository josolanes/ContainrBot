namespace ContainrBot.Services;

public class ContainrBotApiService(
	IConfiguration configuration,
	HttpClient httpClient) : IContainrBotApiService
{
	public async Task<string> ListContainers()
	{
		return await GetRequest("list");
	}

	public async Task<string> StartContainers(string name)
	{
		return await GetRequest("start", name);
	}

	public async Task<string> StopContainers(string name)
	{
		return await GetRequest("stop", name);
	}

	public async Task<string> Restart(string name)
	{
		return await GetRequest("restart", name);
	}

	private async Task<string> GetRequest(string action, string container = "")
	{
		var url = action;

		if (!string.IsNullOrEmpty(container))
		{
			url = $"{url}/{container}";
		}

		httpClient.Timeout = TimeSpan.FromSeconds(60);

		var response = await httpClient.GetAsync(url);

		return await response.Content.ReadAsStringAsync();
	}
}