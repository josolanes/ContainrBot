using ContainrBotApi.Models.Internal;

using Docker.DotNet;
using Docker.DotNet.Models;

namespace ContainrBotApi.Orchestrators;

public class DockerOrchestrator : IOrchestrator
{
	private const string DockerEndpoint = "unix:///var/run/docker.sock";

	private static readonly DockerClient Client = new DockerClientConfiguration(new Uri(DockerEndpoint)).CreateClient();

	public string Name => "Docker";

	public List<string> RequiredContainerProperties { get; } =
	[
		nameof(Container.ContainerName),
		nameof(Container.FriendlyName)
	];

	public async Task<IList<string>> List(IList<Container> containers)
	{
		List<string> output = [];

		foreach (var container in containers)
		{
			try
			{
				var status = await Client.Containers.InspectContainerAsync(container.ContainerName);

				var isRunning = status.State.Running;

				output.Add($"{container.FriendlyName} is {(isRunning ? "running" : "not running")}");
			}
			catch (Exception ex)
			{
				output.Add($"{container.FriendlyName} is not deployed");
			}
		}

		return output;
	}

	public async Task Start(Container container)
	{
		await Client.Containers.StartContainerAsync(container.ContainerName, new ContainerStartParameters());
	}

	public async Task Stop(Container container)
	{
		await Client.Containers.StopContainerAsync(container.ContainerName, new ContainerStopParameters());
	}

	public async Task Restart(Container container)
	{
		await Client.Containers.RestartContainerAsync(container.ContainerName, new ContainerRestartParameters());
	}

	public async Task<bool> Exists(Container container)
	{
		try
		{
			await Client.Containers.InspectContainerAsync(container.ContainerName);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public async Task<bool> CanConnect()
	{
		return await Task.FromResult((await Client.System.GetVersionAsync())?.Components.Count > 0);
	}

	public Task<bool> IsContainerValid(Container container)
	{
		return Task.FromResult(!string.IsNullOrEmpty(container.ContainerName)
									&& !string.IsNullOrEmpty(container.FriendlyName));
	}
}