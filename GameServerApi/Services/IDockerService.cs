namespace GameServerApi.Services;

public interface IDockerService
{
    public string GetContainerId(string containerName);
    public bool StartContainer(string id);
    public bool StopContainer(string id);
}
