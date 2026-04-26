namespace ContainrBot.Services;

public interface IContainrBotApiService
{
	Task<string> StartContainers(string name);

	Task<string> StopContainers(string name);

	Task<string> ListContainers();

	Task<string> Debug();
}