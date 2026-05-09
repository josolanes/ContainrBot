using ContainrBotApi.Models.Internal;

using Docker.DotNet;
using Docker.DotNet.Models;

namespace ContainrBotApi.Orchestrators;

public class DockerOrchestrator(IDockerClient client) : IOrchestrator
{
	private const string DockerEndpoint = "unix:///var/run/docker.sock";

	public string Name => "Docker";

	public List<string> RequiredContainerProperties { get; } =
	[
		nameof(Container.ContainerName),
		nameof(Container.FriendlyName)
	];

	public DockerOrchestrator() : this(new DockerClientConfiguration(new Uri(DockerEndpoint)).CreateClient())
	{
	}

	public async Task<IList<string>> List(IList<Container> containers)
	{
		List<string> output = [];

		foreach (var container in containers)
		{
			try
			{
				var status = await client.Containers.InspectContainerAsync(container.ContainerName);

				output.Add($"{container.FriendlyName} is {(status.State.Running ? "running" : "not running")}");
			}
			catch
			{
				output.Add($"{container.FriendlyName} is not deployed");
			}
		}

		return output;
	}

	public async Task Start(Container container)
	{
		await client.Containers.StartContainerAsync(container.ContainerName, new ContainerStartParameters());
	}

	public async Task Stop(Container container)
	{
		await client.Containers.StopContainerAsync(container.ContainerName, new ContainerStopParameters());
	}

	public async Task Restart(Container container)
	{
		await client.Containers.RestartContainerAsync(container.ContainerName, new ContainerRestartParameters());
	}

	public async Task<bool> Exists(Container container)
	{
		try
		{
			await client.Containers.InspectContainerAsync(container.ContainerName);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public async Task<bool> CanConnect()
	{
		try
		{
			return await Task.FromResult((await client.System.GetVersionAsync())?.Components.Count > 0);
		}
		catch
		{
			return false;
		}
	}

	public Task<bool> IsContainerValid(Container container)
	{
		return Task.FromResult(!string.IsNullOrEmpty(container.ContainerName)
									&& !string.IsNullOrEmpty(container.FriendlyName));
	}
}