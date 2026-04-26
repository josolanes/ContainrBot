namespace ContainrBot.Services.Chatbots;

public interface IChatbot
{
	public const string CommandName = "containr";
	public const string CommandDescription = "containr commands!";

	public const string ListName = "list";
	public const string ListDescription = "List of available containers";

	public const string StartName = "start";
	public const string StartDescription = "Starts a container";

	public const string StopName = "stop";
	public const string StopDescription = "Stops a container";

	public const string RestartName = "restart";
	public const string RestartDescription = "Restarts a container";

	public const string InProgressMessage = "Working on it...";

	Task<string> List();

	Task<string> Start(string container);

	Task<string> Stop(string container);
	
	Task<string> Restart(string container);
}