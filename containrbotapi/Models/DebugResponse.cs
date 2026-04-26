using ContainrBotApi.Models.Internal;

namespace ContainrBotApi.Models;

public class DebugResponse
{
	public bool IsOrchestratorAccessible { get; set; }

	public string? OrchestratorConnectionError { get; set; }

	public IList<Container>? Containers { get; set; }

	public IList<string>? ContainersStatus { get; set; }
}