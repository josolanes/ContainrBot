using ContainrBotApi.Models.Internal;

namespace ContainrBotApi.Orchestrators;

public interface IOrchestrator
{
	string Name { get; }

	Task<IList<string>> List(IList<Container> containers);

	Task Start(Container container);

	Task Stop(Container container);

	Task Restart(Container container);

	Task<bool> Exists(Container container);

	Task<bool> CanConnect();
}