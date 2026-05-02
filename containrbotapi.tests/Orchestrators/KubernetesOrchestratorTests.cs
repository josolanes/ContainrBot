using System.Reflection;

using ContainrBotApi.Models.Internal;
using ContainrBotApi.Orchestrators;

using k8s;
using k8s.Autorest;
using k8s.Models;

using Moq;

namespace ContainrBotApi.Tests.Orchestrators;

public class KubernetesOrchestratorTests
{
	[Test]
	public void Name()
	{
		var orchestrator = new KubernetesOrchestrator(It.IsAny<IKubernetes>());
		
		var expected = orchestrator.Name.Replace("Orchestrator", string.Empty);
		
		Assert.That(orchestrator.Name, Is.EqualTo(expected));
	}
	
	[Test]
	public void RequiredContainerProperties()
	{
		var orchestrator = new KubernetesOrchestrator(It.IsAny<IKubernetes>());
		
		Assert.That(orchestrator.RequiredContainerProperties, Is.EqualTo([
			nameof(Container.ContainerName),
			nameof(Container.FriendlyName),
			nameof(Container.Namespace)
		]));
	}
	
	[TestCase(1)]
	[TestCase(100)]
	public async Task List_IsRunning(int replicas)
	{
		var kubernetesClient = new Mock<IKubernetes>();
		kubernetesClient.Setup(s => s.AppsV1.ReadNamespacedDeploymentScaleWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>()))
			.ReturnsAsync(new HttpOperationResponse<V1Scale>()
			{
				Body = new V1Scale()
				{
					Spec = new V1ScaleSpec()
					{
						Replicas = replicas
					}
				}
			});

		var orchestrator = new KubernetesOrchestrator(kubernetesClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer",
			Namespace = "SomeNamespace"
		};

		var result = await orchestrator.List([container]);
		
		Assert.That(result, Is.EqualTo([ $"{container.FriendlyName} is running" ]));
	}
	
	[Test]
	public async Task List_IsNotRunning()
	{
		var kubernetesClient = new Mock<IKubernetes>();
		kubernetesClient.Setup(s => s.AppsV1.ReadNamespacedDeploymentScaleWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>()))
			.ReturnsAsync(new HttpOperationResponse<V1Scale>()
			{
				Body = new V1Scale()
				{
					Spec = new V1ScaleSpec()
					{
						Replicas = 0
					}
				}
			});

		var orchestrator = new KubernetesOrchestrator(kubernetesClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer",
			Namespace = "SomeNamespace"
		};

		var result = await orchestrator.List([container]);
		
		Assert.That(result, Is.EqualTo([ $"{container.FriendlyName} is not running" ]));
	}
	
	[Test]
	public async Task List_IsNotDeployed()
	{
		var kubernetesClient = new Mock<IKubernetes>();
		kubernetesClient.Setup(s => s.AppsV1.ReadNamespacedDeploymentScaleWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>()))
			.ThrowsAsync(new Exception());

		var orchestrator = new KubernetesOrchestrator(kubernetesClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer",
			Namespace = "SomeNamespace"
		};

		var result = await orchestrator.List([container]);
		
		Assert.That(result, Is.EqualTo([ $"{container.FriendlyName} is not deployed" ]));
	}
	
	[Test]
	public void Start_Success()
	{
		var kubernetesClient = new Mock<IKubernetes>();
		kubernetesClient.Setup(s =>
			s.AppsV1.PatchNamespacedDeploymentScaleWithHttpMessagesAsync(It.IsAny<V1Patch>(), It.IsAny<string>(), It.IsAny<string>()));

		var orchestrator = new KubernetesOrchestrator(kubernetesClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer",
			Namespace = "SomeNamespace"
		};
		
		Assert.DoesNotThrowAsync(async () => await orchestrator.Start(container));
	}
	
	[Test]
	public void Start_Throws()
	{
		var kubernetesClient = new Mock<IKubernetes>();
		kubernetesClient.Setup(s =>
				s.AppsV1.PatchNamespacedDeploymentScaleWithHttpMessagesAsync(It.IsAny<V1Patch>(), It.IsAny<string>(), It.IsAny<string>()))
			.ThrowsAsync(new Exception());

		var orchestrator = new KubernetesOrchestrator(kubernetesClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer",
			Namespace = "SomeNamespace"
		};
		
		Assert.CatchAsync(async () => await orchestrator.Start(container));
	}
	
	[Test]
	public void Stop_Success()
	{
		var kubernetesClient = new Mock<IKubernetes>();
		kubernetesClient.Setup(s =>
			s.AppsV1.PatchNamespacedDeploymentScaleWithHttpMessagesAsync(It.IsAny<V1Patch>(), It.IsAny<string>(), It.IsAny<string>()));

		var orchestrator = new KubernetesOrchestrator(kubernetesClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer",
			Namespace = "SomeNamespace"
		};
		
		Assert.DoesNotThrowAsync(async () => await orchestrator.Stop(container));
	}
	
	[Test]
	public void Stop_Throws()
	{
		var kubernetesClient = new Mock<IKubernetes>();
		kubernetesClient.Setup(s =>
				s.AppsV1.PatchNamespacedDeploymentScaleWithHttpMessagesAsync(It.IsAny<V1Patch>(), It.IsAny<string>(), It.IsAny<string>()))
			.ThrowsAsync(new Exception());

		var orchestrator = new KubernetesOrchestrator(kubernetesClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer",
			Namespace = "SomeNamespace"
		};
		
		Assert.CatchAsync(async () => await orchestrator.Stop(container));
	}
	
	[Test]
	public void Restart_Success()
	{	
		var kubernetesClient = new Mock<IKubernetes>();
		kubernetesClient.Setup(s =>
			s.AppsV1.PatchNamespacedDeploymentScaleWithHttpMessagesAsync(It.IsAny<V1Patch>(), It.IsAny<string>(), It.IsAny<string>()));
		
		var orchestrator = new KubernetesOrchestrator(kubernetesClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer",
			Namespace = "SomeNamespace"
		};
		
		Assert.DoesNotThrowAsync(async () => await orchestrator.Restart(container));
	}
	
	[Test]
	public void Restart_Throws()
	{
		var kubernetesClient = new Mock<IKubernetes>();
		kubernetesClient.Setup(s =>
			s.AppsV1.PatchNamespacedDeploymentScaleWithHttpMessagesAsync(It.IsAny<V1Patch>(), It.IsAny<string>(), It.IsAny<string>()))
			.ThrowsAsync(new Exception());
		
		var orchestrator = new KubernetesOrchestrator(kubernetesClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer",
			Namespace = "SomeNamespace"
		};
		
		Assert.CatchAsync(async () => await orchestrator.Restart(container));
	}
	
	[Test]
	public void Exists_ReturnsTrue()
	{
		var kubernetesClient = new Mock<IKubernetes>();
		kubernetesClient.Setup(s =>
			s.AppsV1.ReadNamespacedDeploymentWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>()));
		
		var orchestrator = new KubernetesOrchestrator(kubernetesClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer",
			Namespace = "SomeNamespace"
		};

		Assert.DoesNotThrowAsync(async() => await orchestrator.Exists(container));
		Assert.ThatAsync(async() => await orchestrator.Exists(container), Is.True);
	}
	
	[Test]
	public void Exists_ReturnsFalse()
	{
		var kubernetesClient = new Mock<IKubernetes>();
		kubernetesClient.Setup(s =>
			s.AppsV1.ReadNamespacedDeploymentWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>()))
			.ThrowsAsync(new  Exception());
		
		var orchestrator = new KubernetesOrchestrator(kubernetesClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer",
			Namespace = "SomeNamespace"
		};
		
		Assert.DoesNotThrowAsync(async() => await orchestrator.Exists(container));
		Assert.ThatAsync(async() => await orchestrator.Exists(container), Is.False);
	}
	
	[Test]
	public void CanConnect_ReturnsTrue()
	{
		var kubernetesClient = new Mock<IKubernetes>();
		kubernetesClient.Setup(s => s.CoreV1.ListNodeWithHttpMessagesAsync())
			.ReturnsAsync(new HttpOperationResponse<V1NodeList>()
			{
				Body = new V1NodeList()
				{
					Items = [ new V1Node() ]
				}
			});
		
		var orchestrator = new KubernetesOrchestrator(kubernetesClient.Object);
		
		Assert.DoesNotThrowAsync(async () => await orchestrator.CanConnect());
		Assert.ThatAsync(async() => await orchestrator.CanConnect(), Is.True);
	}
	
	[Test]
	public void CanConnect_Exception_ReturnsFalse()
	{
		var kubernetesClient = new Mock<IKubernetes>();
		kubernetesClient.Setup(s => s.CoreV1.ListNodeWithHttpMessagesAsync())
			.ThrowsAsync(new Exception());
		
		var orchestrator = new KubernetesOrchestrator(kubernetesClient.Object);
		
		Assert.DoesNotThrowAsync(async () => await orchestrator.CanConnect());
		Assert.ThatAsync(async() => await orchestrator.CanConnect(), Is.False);
	}
	
	[Test]
	public void CanConnect_ReturnsFalse()
	{
		var kubernetesClient = new Mock<IKubernetes>();
		kubernetesClient.Setup(s => s.CoreV1.ListNodeWithHttpMessagesAsync())
			.ReturnsAsync(new HttpOperationResponse<V1NodeList>()
			{
				Body = new V1NodeList()
				{
					Items = []
				}
			});
		
		var orchestrator = new KubernetesOrchestrator(kubernetesClient.Object);
		
		Assert.DoesNotThrowAsync(async () => await orchestrator.CanConnect());
		Assert.ThatAsync(async() => await orchestrator.CanConnect(), Is.False);
	}
	
	[Test]
	public async Task IsContainerValid_ReturnsTrue()
	{
		var kubernetesClient = new Mock<IKubernetes>();
		
		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer",
			Namespace = "SomeNamespace"
		};
		
		var orchestrator = new KubernetesOrchestrator(kubernetesClient.Object);
		var isValid = await orchestrator.IsContainerValid(container);
		
		Assert.That(isValid, Is.True);
	}
	
	[Test]
	public async Task IsContainerValid_ReturnsFalse()
	{
		var kubernetesClient = new Mock<IKubernetes>();
		
		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer"
		};
		
		var orchestrator = new KubernetesOrchestrator(kubernetesClient.Object);
		var isValid = await orchestrator.IsContainerValid(container);
		
		Assert.That(isValid, Is.False);
	}
}