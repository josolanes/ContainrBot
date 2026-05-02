using System.Net;
using System.Text.Json;

using ContainrBotApi.Models.Internal;

using Json.Patch;

using k8s;
using k8s.Autorest;
using k8s.Models;

using Microsoft.AspNetCore.JsonPatch;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ContainrBotApi.Orchestrators;

public class KubernetesOrchestrator(IKubernetes client) : IOrchestrator
{
	public string Name => "Kubernetes";

	public List<string> RequiredContainerProperties { get; } =
	[
		nameof(Container.ContainerName),
		nameof(Container.FriendlyName),
		nameof(Container.Namespace)
	];
	
	public KubernetesOrchestrator() : this(new Kubernetes(KubernetesClientConfiguration.InClusterConfig()))
	{
	}

	public async Task<IList<string>> List(IList<Container> containers)
	{
		List<string> output = [];

		foreach (var container in containers)
		{
			try
			{
				var scale = await client.AppsV1.ReadNamespacedDeploymentScaleWithHttpMessagesAsync(container.ContainerName, container.Namespace);
				var isRunning = scale.Body.Spec.Replicas is > 0;

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

		await client.AppsV1.PatchNamespacedDeploymentScaleWithHttpMessagesAsync(
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

		await client.AppsV1.PatchNamespacedDeploymentScaleWithHttpMessagesAsync(
			v1Patch,
			container.ContainerName,
			container.Namespace);
	}

	public async Task Restart(Container container)
	{
		var patch = new JsonPatchDocument<V1Deployment>();
		patch.Replace(d => d.Spec.Template.Metadata.Annotations, new Dictionary<string, string>
		{
			["kubectl.kubernetes.io/restartedAt"] = DateTime.UtcNow.ToString("s")
		});
		
		var jsonPatchString = JsonConvert.SerializeObject(patch);
		
		await client.AppsV1.PatchNamespacedDeploymentWithHttpMessagesAsync(new V1Patch(jsonPatchString, V1Patch.PatchType.JsonPatch),
			container.ContainerName, container.Namespace);
	}

	public async Task<bool> Exists(Container container)
	{
		try
		{
			await client.AppsV1.ReadNamespacedDeploymentWithHttpMessagesAsync(container.ContainerName, container.Namespace);
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
			return await Task.FromResult((await client.CoreV1.ListNodeWithHttpMessagesAsync())?.Body.Items?.Any() ?? false);
		}
		catch
		{
			return false;
		}
	}

	public Task<bool> IsContainerValid(Container container)
	{
		return Task.FromResult(!string.IsNullOrEmpty(container.ContainerName)
							   && !string.IsNullOrEmpty(container.FriendlyName)
							   && !string.IsNullOrEmpty(container.Namespace));
	}
}