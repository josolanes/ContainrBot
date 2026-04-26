using System.Text.Json;
using ContainrBotApi.Models.Internal;
using Json.Patch;
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
		var deployment = await Client.AppsV1.ReadNamespacedDeploymentAsync(container.ContainerName, container.Namespace).ConfigureAwait(false);
		var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
		var old = System.Text.Json.JsonSerializer.SerializeToDocument(deployment, options);
		var now = DateTimeOffset.Now.ToUnixTimeSeconds();
		var restart = new Dictionary<string, string>
		{
			["date"] = now.ToString(),
		};

		deployment.Spec.Template.Metadata.Annotations = restart;

		var expected = System.Text.Json.JsonSerializer.SerializeToDocument(deployment);

		var patch = old.CreatePatch(expected);
		await Client.AppsV1.PatchNamespacedDeploymentAsync(new V1Patch(patch, V1Patch.PatchType.JsonPatch), container.ContainerName, container.Namespace).ConfigureAwait(false);
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