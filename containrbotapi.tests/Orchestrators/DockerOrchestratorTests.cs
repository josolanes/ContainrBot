using ContainrBotApi.Models.Internal;
using ContainrBotApi.Orchestrators;

using Docker.DotNet;
using Docker.DotNet.Models;

using Moq;

namespace ContainrBotApi.Tests.Orchestrators;

public class DockerOrchestratorTests
{
	[Test]
	public void Name()
	{
		var orchestrator = new DockerOrchestrator(It.IsAny<IDockerClient>());

		var expected = orchestrator.Name.Replace("Orchestrator", string.Empty);

		Assert.That(orchestrator.Name, Is.EqualTo(expected));
	}

	[Test]
	public void RequiredContainerProperties()
	{
		var orchestrator = new DockerOrchestrator(It.IsAny<IDockerClient>());

		Assert.That(orchestrator.RequiredContainerProperties, Is.EqualTo([
			nameof(Container.ContainerName),
			nameof(Container.FriendlyName)
		]));
	}

	[Test]
	public async Task List_IsRunning()
	{
		var dockerContainerOperations = new Mock<IContainerOperations>();
		dockerContainerOperations.Setup(s => s.InspectContainerAsync(It.IsAny<string>()))
			.ReturnsAsync(new ContainerInspectResponse()
			{
				State = new ContainerState()
				{
					Running = true
				}
			});

		var dockerClient = new Mock<IDockerClient>();
		dockerClient.Setup(s => s.Containers)
			.Returns(dockerContainerOperations.Object);

		var orchestrator = new DockerOrchestrator(dockerClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer"
		};

		var result = await orchestrator.List([container]);

		Assert.That(result, Is.EqualTo([$"{container.FriendlyName} is running"]));
	}

	[Test]
	public async Task List_IsNotRunning()
	{
		var dockerContainerOperations = new Mock<IContainerOperations>();
		dockerContainerOperations.Setup(s => s.InspectContainerAsync(It.IsAny<string>()))
			.ReturnsAsync(new ContainerInspectResponse()
			{
				State = new ContainerState()
				{
					Running = false
				}
			});

		var dockerClient = new Mock<IDockerClient>();
		dockerClient.Setup(s => s.Containers)
			.Returns(dockerContainerOperations.Object);

		var orchestrator = new DockerOrchestrator(dockerClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer"
		};

		var result = await orchestrator.List([container]);

		Assert.That(result, Is.EqualTo([$"{container.FriendlyName} is not running"]));
	}

	[Test]
	public async Task List_IsNotDeployed()
	{
		var dockerContainerOperations = new Mock<IContainerOperations>();
		dockerContainerOperations.Setup(s => s.InspectContainerAsync(It.IsAny<string>()))
			.ThrowsAsync(new Exception());

		var dockerClient = new Mock<IDockerClient>();
		dockerClient.Setup(s => s.Containers)
			.Returns(dockerContainerOperations.Object);

		var orchestrator = new DockerOrchestrator(dockerClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer"
		};

		var result = await orchestrator.List([container]);

		Assert.That(result, Is.EqualTo([$"{container.FriendlyName} is not deployed"]));
	}

	[Test]
	public void Start_Success()
	{
		var dockerContainerOperations = new Mock<IContainerOperations>();
		dockerContainerOperations.Setup(s => s.StartContainerAsync(It.IsAny<string>(), It.IsAny<ContainerStartParameters>()))
			.ReturnsAsync(true);

		var dockerClient = new Mock<IDockerClient>();
		dockerClient.Setup(s => s.Containers)
			.Returns(dockerContainerOperations.Object);

		var orchestrator = new DockerOrchestrator(dockerClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer"
		};

		Assert.DoesNotThrowAsync(async () => await orchestrator.Start(container));
	}

	[Test]
	public void Start_Throws()
	{
		var dockerContainerOperations = new Mock<IContainerOperations>();
		dockerContainerOperations.Setup(s => s.StartContainerAsync(It.IsAny<string>(), It.IsAny<ContainerStartParameters>()))
			.ThrowsAsync(new Exception());

		var dockerClient = new Mock<IDockerClient>();
		dockerClient.Setup(s => s.Containers)
			.Returns(dockerContainerOperations.Object);

		var orchestrator = new DockerOrchestrator(dockerClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer"
		};

		Assert.CatchAsync(async () => await orchestrator.Start(container));
	}

	[Test]
	public void Stop_Success()
	{
		var dockerContainerOperations = new Mock<IContainerOperations>();
		dockerContainerOperations.Setup(s => s.StopContainerAsync(It.IsAny<string>(), It.IsAny<ContainerStopParameters>()))
			.ReturnsAsync(true);

		var dockerClient = new Mock<IDockerClient>();
		dockerClient.Setup(s => s.Containers)
			.Returns(dockerContainerOperations.Object);

		var orchestrator = new DockerOrchestrator(dockerClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer"
		};

		Assert.DoesNotThrowAsync(async () => await orchestrator.Stop(container));
	}

	[Test]
	public void Stop_Throws()
	{
		var dockerContainerOperations = new Mock<IContainerOperations>();
		dockerContainerOperations.Setup(s => s.StopContainerAsync(It.IsAny<string>(), It.IsAny<ContainerStopParameters>()))
			.ThrowsAsync(new Exception());

		var dockerClient = new Mock<IDockerClient>();
		dockerClient.Setup(s => s.Containers)
			.Returns(dockerContainerOperations.Object);

		var orchestrator = new DockerOrchestrator(dockerClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer"
		};

		Assert.CatchAsync(async () => await orchestrator.Stop(container));
	}

	[Test]
	public void Restart_Success()
	{
		var dockerContainerOperations = new Mock<IContainerOperations>();
		dockerContainerOperations.Setup(s =>
			s.RestartContainerAsync(It.IsAny<string>(), It.IsAny<ContainerRestartParameters>()));

		var dockerClient = new Mock<IDockerClient>();
		dockerClient.Setup(s => s.Containers)
			.Returns(dockerContainerOperations.Object);

		var orchestrator = new DockerOrchestrator(dockerClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer"
		};

		Assert.DoesNotThrowAsync(async () => await orchestrator.Restart(container));
	}

	[Test]
	public void Restart_Throws()
	{
		var dockerContainerOperations = new Mock<IContainerOperations>();
		dockerContainerOperations.Setup(s => s.RestartContainerAsync(It.IsAny<string>(), It.IsAny<ContainerRestartParameters>()))
			.ThrowsAsync(new Exception());

		var dockerClient = new Mock<IDockerClient>();
		dockerClient.Setup(s => s.Containers)
			.Returns(dockerContainerOperations.Object);

		var orchestrator = new DockerOrchestrator(dockerClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer"
		};

		Assert.CatchAsync(async () => await orchestrator.Restart(container));
	}

	[Test]
	public void Exists_ReturnsTrue()
	{
		var dockerContainerOperations = new Mock<IContainerOperations>();
		dockerContainerOperations.Setup(s => s.InspectContainerAsync(It.IsAny<string>()))
			.ReturnsAsync(new ContainerInspectResponse());

		var dockerClient = new Mock<IDockerClient>();
		dockerClient.Setup(s => s.Containers)
			.Returns(dockerContainerOperations.Object);

		var orchestrator = new DockerOrchestrator(dockerClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer"
		};

		Assert.DoesNotThrowAsync(async () => await orchestrator.Exists(container));
		Assert.ThatAsync(async () => await orchestrator.Exists(container), Is.True);
	}

	[Test]
	public void Exists_ReturnsFalse()
	{
		var dockerContainerOperations = new Mock<IContainerOperations>();
		dockerContainerOperations.Setup(s => s.InspectContainerAsync(It.IsAny<string>()))
			.ThrowsAsync(new Exception());

		var dockerClient = new Mock<IDockerClient>();
		dockerClient.Setup(s => s.Containers)
			.Returns(dockerContainerOperations.Object);

		var orchestrator = new DockerOrchestrator(dockerClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer"
		};

		Assert.DoesNotThrowAsync(async () => await orchestrator.Exists(container));
		Assert.ThatAsync(async () => await orchestrator.Exists(container), Is.False);
	}

	[Test]
	public void CanConnect_ReturnsTrue()
	{
		var dockerSystemOperations = new Mock<ISystemOperations>();
		dockerSystemOperations.Setup(s => s.GetVersionAsync())
			.ReturnsAsync(new VersionResponse()
			{
				Components = [new ComponentVersion() { Name = It.IsAny<string>() }]
			});

		var dockerClient = new Mock<IDockerClient>();
		dockerClient.Setup(s => s.System)
			.Returns(dockerSystemOperations.Object);

		var orchestrator = new DockerOrchestrator(dockerClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer"
		};

		Assert.DoesNotThrowAsync(async () => await orchestrator.CanConnect());
		Assert.ThatAsync(async () => await orchestrator.CanConnect(), Is.True);
	}

	[Test]
	public void CanConnect_Exception_ReturnsFalse()
	{
		var dockerSystemOperations = new Mock<ISystemOperations>();
		dockerSystemOperations.Setup(s => s.GetVersionAsync())
			.ThrowsAsync(new Exception());

		var dockerClient = new Mock<IDockerClient>();
		dockerClient.Setup(s => s.System)
			.Returns(dockerSystemOperations.Object);

		var orchestrator = new DockerOrchestrator(dockerClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer"
		};

		Assert.DoesNotThrowAsync(async () => await orchestrator.CanConnect());
		Assert.ThatAsync(async () => await orchestrator.CanConnect(), Is.False);
	}

	[Test]
	public void CanConnect_ReturnsFalse()
	{
		var dockerSystemOperations = new Mock<ISystemOperations>();
		dockerSystemOperations.Setup(s => s.GetVersionAsync())
			.ReturnsAsync(new VersionResponse()
			{
				Components = []
			});

		var dockerClient = new Mock<IDockerClient>();
		dockerClient.Setup(s => s.System)
			.Returns(dockerSystemOperations.Object);

		var orchestrator = new DockerOrchestrator(dockerClient.Object);

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer"
		};

		Assert.DoesNotThrowAsync(async () => await orchestrator.CanConnect());
		Assert.ThatAsync(async () => await orchestrator.CanConnect(), Is.False);
	}

	[Test]
	public async Task IsContainerValid_ReturnsTrue()
	{
		var dockerClient = new Mock<IDockerClient>();

		var container = new Container()
		{
			FriendlyName = "SomeContainer",
			ContainerName = "SomeContainer"
		};

		var orchestrator = new DockerOrchestrator(dockerClient.Object);
		var isValid = await orchestrator.IsContainerValid(container);

		Assert.That(isValid, Is.True);
	}

	[Test]
	public async Task IsContainerValid_ReturnsFalse()
	{
		var dockerClient = new Mock<IDockerClient>();

		var container = new Container()
		{
			FriendlyName = "SomeContainer"
		};

		var orchestrator = new DockerOrchestrator(dockerClient.Object);
		var isValid = await orchestrator.IsContainerValid(container);

		Assert.That(isValid, Is.False);
	}
}