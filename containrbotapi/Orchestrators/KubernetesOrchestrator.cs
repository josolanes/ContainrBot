using ContainrBotApi.Models.Internal;
using k8s;
using k8s.Autorest;
using k8s.Models;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ContainrBotApi.Orchestrators;

public class KubernetesOrchestrator : IOrchestrator
{
	private static readonly KubernetesClientConfiguration K8SConfig = KubernetesClientConfiguration.InClusterConfig();
	private static readonly Kubernetes Client = new(K8SConfig);

	public string Name { get; } = "Kubernetes";

	public List<string> RequiredContainerProperties { get; } =
	[
		nameof(Container.ContainerName),
		nameof(Container.FriendlyName),
		nameof(Container.Namespace)
	];

	public async Task<IList<string>> List(IList<Container> containers)
	{
		List<string> output = [];

		foreach (var container in containers)
		{
			try
			{
				var scale = await Client.ReadNamespacedDeploymentScaleAsync(container.ContainerName, container.Namespace);
				var isRunning = scale.Spec.Replicas is > 0;

				output.Add($"{container.FriendlyName} is {(isRunning ? "running" : "not running")}");
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
		var patch = new JsonPatchDocument<V1Scale>
		{
			ContractResolver = new DefaultContractResolver
			{
				NamingStrategy = new CamelCaseNamingStrategy()
			}
		};
		patch.Replace(e => e.Spec.Replicas, 1);

		var jsonPatchString = JsonConvert.SerializeObject(patch);

		var v1Patch = new V1Patch(jsonPatchString, V1Patch.PatchType.JsonPatch);

		await Client.AppsV1.PatchNamespacedDeploymentScaleAsync(
			v1Patch,
			container.ContainerName,
			container.Namespace);
	}

	public async Task Stop(Container container)
	{
		var patch = new JsonPatchDocument<V1Scale>
		{
			ContractResolver = new DefaultContractResolver
			{
				NamingStrategy = new CamelCaseNamingStrategy()
			}
		};
		patch.Replace(e => e.Spec.Replicas, 0);

		var jsonPatchString = JsonConvert.SerializeObject(patch);

		var v1Patch = new V1Patch(jsonPatchString, V1Patch.PatchType.JsonPatch);

		await Client.AppsV1.PatchNamespacedDeploymentScaleAsync(
			v1Patch,
			container.ContainerName,
			container.Namespace);
	}

	public async Task Restart(Container container)
	{
		var deployment = await Client.ReadNamespacedDeploymentAsync(container.ContainerName, container.Namespace);
		var restart = new Dictionary<string, string>
		{
			["kubectl.kubernetes.io/restartedAt"] = DateTime.UtcNow.ToString("s")
		};

		var patch = new JsonPatchDocument<V1Deployment>();
		patch.Replace(e => e.Spec.Template.Metadata.Annotations, restart);

		var jsonPatchString = JsonConvert.SerializeObject(patch);

		await Client.AppsV1.PatchNamespacedDeploymentScaleAsync(
			new V1Patch(jsonPatchString, V1Patch.PatchType.JsonPatch),
			container.ContainerName,
			container.Namespace);
	}

	public async Task<bool> Exists(Container container)
	{
		try
		{
			await Client.AppsV1.ReadNamespacedDeploymentAsync(container.ContainerName, container.Namespace);
			return true;
		}
		catch (HttpOperationException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
		{
			return false;
		}
	}

	public async Task<bool> CanConnect()
	{
		return await Task.FromResult((await Client.ListNodeAsync())?.Items?.Any() ?? false);
	}

	public Task<bool> IsContainerValid(Container container)
	{
		return Task.FromResult(!string.IsNullOrEmpty(container.ContainerName)
		       && !string.IsNullOrEmpty(container.FriendlyName)
		       && !string.IsNullOrEmpty(container.ContainerName));
	}
}