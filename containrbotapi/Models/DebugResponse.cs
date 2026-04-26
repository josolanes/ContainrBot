using System.Text.Json.Serialization;
using ContainrBotApi.Models.Internal;

namespace ContainrBotApi.Models;

public class DebugResponse
{
	[JsonPropertyOrder(1)]
	public string CurrentOrchestrator { get; set; } = "***NOT SET***";
	
	[JsonPropertyOrder(2)]
	public IList<string> SupportedOrchestrators { get; set; } = [];
	
	[JsonPropertyOrder(3)]
	public bool IsOrchestratorAccessible { get; set; }

	[JsonPropertyOrder(4)]
	public string? OrchestratorConnectionError { get; set; }

	[JsonPropertyOrder(5)]
	public IList<Container>? Containers { get; set; }

	[JsonPropertyOrder(6)]
	public IList<string>? ContainersStatus { get; set; }
}