namespace ContainrBot.Services.Chatbots;

public interface IChatbot
{
	const string CommandName = "containr";
	const string CommandDescription = "containr commands!";

	const string ListName = "list";
	const string ListDescription = "List of available containers";

	const string StartName = "start";
	const string StartDescription = "Starts a container";

	const string StopName = "stop";
	const string StopDescription = "Stops a container";

	const string RestartName = "restart";
	const string RestartDescription = "Restarts a container";

	const string InProgressMessage = "Working on it...";

	Task<string> List();

	Task<string> Start(string container);

	Task<string> Stop(string container);

	Task<string> Restart(string container);
}