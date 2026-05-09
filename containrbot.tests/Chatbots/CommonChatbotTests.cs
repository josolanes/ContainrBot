using System.Text.Json;

using ContainrBot.Services;
using ContainrBot.Services.Chatbots;

using Moq;

namespace ContainrBot.Tests.Chatbots;

[TestFixture(typeof(DiscordChatbot))]
public class CommonChatbotTests<T> where T : IChatbot
{
	private const string containerName = "A Container";

	[Test]
	public async Task List_Success()
	{
		var containrBotApiService = new Mock<IContainrBotApiService>();
		containrBotApiService.Setup(s => s.ListContainers())
			.ReturnsAsync(JsonSerializer.Serialize<List<string>>([containerName]));

		var chatbot = (T)Activator.CreateInstance(typeof(T), [containrBotApiService.Object])!;

		var result = await chatbot.List();

		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.EqualTo(containerName));
		containrBotApiService.Verify(s => s.ListContainers(), Times.Once);
	}

	[Test]
	public async Task List_Error()
	{
		var containrBotApiService = new Mock<IContainrBotApiService>();
		containrBotApiService.Setup(s => s.ListContainers())
			.ThrowsAsync(new Exception("Some exception"));

		var chatbot = (T)Activator.CreateInstance(typeof(T), [containrBotApiService.Object])!;

		var result = await chatbot.List();

		Assert.That(result, Is.Not.Null);
		Assert.That(result, Does.StartWith("Unable to list containers:"));
		containrBotApiService.Verify(s => s.ListContainers(), Times.Once);
	}

	[Test]
	public async Task Start_Success()
	{
		var containrBotApiService = new Mock<IContainrBotApiService>();
		containrBotApiService.Setup(s => s.StartContainers(containerName))
			.ReturnsAsync(JsonSerializer.Serialize<string>($"Successfully started {containerName}!"));

		var chatbot = (T)Activator.CreateInstance(typeof(T), [containrBotApiService.Object])!;

		var result = await chatbot.Start(containerName);

		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.EqualTo($"Successfully started {containerName}!"));
		containrBotApiService.Verify(s => s.StartContainers(containerName), Times.Once);
	}

	[Test]
	public async Task Start_Error()
	{
		var containrBotApiService = new Mock<IContainrBotApiService>();
		containrBotApiService.Setup(s => s.StartContainers(containerName))
			.ThrowsAsync(new Exception("Some exception"));

		var chatbot = (T)Activator.CreateInstance(typeof(T), [containrBotApiService.Object])!;

		var result = await chatbot.Start(containerName);

		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.EqualTo($"Unable to start container {containerName}: Some exception"));
		containrBotApiService.Verify(s => s.StartContainers(containerName), Times.Once);
	}

	[Test]
	public async Task Stop_Success()
	{
		var containrBotApiService = new Mock<IContainrBotApiService>();
		containrBotApiService.Setup(s => s.StopContainers(containerName))
			.ReturnsAsync(JsonSerializer.Serialize<string>($"Successfully stopped {containerName}!"));

		var chatbot = (T)Activator.CreateInstance(typeof(T), [containrBotApiService.Object])!;

		var result = await chatbot.Stop(containerName);

		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.EqualTo($"Successfully stopped {containerName}!"));
		containrBotApiService.Verify(s => s.StopContainers(containerName), Times.Once);
	}

	[Test]
	public async Task Stop_Error()
	{
		var containrBotApiService = new Mock<IContainrBotApiService>();
		containrBotApiService.Setup(s => s.StopContainers(containerName))
			.ThrowsAsync(new Exception("Some exception"));

		var chatbot = (T)Activator.CreateInstance(typeof(T), [containrBotApiService.Object])!;

		var result = await chatbot.Stop(containerName);

		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.EqualTo($"Unable to stop container {containerName}: Some exception"));
		containrBotApiService.Verify(s => s.StopContainers(containerName), Times.Once);
	}

	[Test]
	public async Task Restart_Success()
	{
		var containrBotApiService = new Mock<IContainrBotApiService>();
		containrBotApiService.Setup(s => s.Restart(containerName))
			.ReturnsAsync(JsonSerializer.Serialize<string>($"Successfully restarted {containerName}!"));

		var chatbot = (T)Activator.CreateInstance(typeof(T), [containrBotApiService.Object])!;

		var result = await chatbot.Restart(containerName);

		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.EqualTo($"Successfully restarted {containerName}!"));
		containrBotApiService.Verify(s => s.Restart(containerName), Times.Once);
	}

	[Test]
	public async Task Restart_Error()
	{
		var containrBotApiService = new Mock<IContainrBotApiService>();
		containrBotApiService.Setup(s => s.Restart(containerName))
			.ThrowsAsync(new Exception("Some exception"));

		var chatbot = (T)Activator.CreateInstance(typeof(T), [containrBotApiService.Object])!;

		var result = await chatbot.Restart(containerName);

		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.EqualTo($"Unable to restart container {containerName}: Some exception"));
		containrBotApiService.Verify(s => s.Restart(containerName), Times.Once);
	}
}