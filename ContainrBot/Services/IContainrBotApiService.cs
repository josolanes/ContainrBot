namespace ContainrBot.Services;

public interface IContainrBotApiService
{
    public string StartContainers(string name, out bool success);

    public string StopContainers(string name, out bool success);

    public string ListContainers(out bool success);
}
