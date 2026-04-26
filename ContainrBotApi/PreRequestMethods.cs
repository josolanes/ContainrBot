using ContainrBotApi.Models.Internal;
using ContainrBotApi.Orchestrators;

namespace ContainrBotApi;

public static class PreRequestMethods
{
	public static async ValueTask<object?> CanConnectToOrchestrator(EndpointFilterInvocationContext invocationContext,
		EndpointFilterDelegate next)
	{
		var orchestrator = invocationContext.GetArgument<IOrchestrator>(0);
		var canConnect = false;
		Exception? exception = null;

		try
		{
			canConnect = await orchestrator.CanConnect();
		}
		catch (Exception ex)
		{
			canConnect = false;
			exception = ex;
		}
		
		if (!canConnect)
		{
			return Results.Problem(
				title: $"Unable to connect to {orchestrator.Name}",
				statusCode: StatusCodes.Status401Unauthorized,
				detail: $"{exception?.Message} {exception?.InnerException?.Message}");
		}

		return await next(invocationContext);
	}

	public static async ValueTask<object?> RequestedContainerInEnvVar(
		EndpointFilterInvocationContext invocationContext,
		EndpointFilterDelegate next,
		List<Container> containers)
	{
		var container = invocationContext.GetArgument<string>(0);

		if (!containers.Exists(e => e.FriendlyName == container))
		{
			return Results.BadRequest($"The container '{container}' is not a valid container.");
		}

		return await next(invocationContext);
	}
}