using ContainrBotApi.Models.Internal;
using ContainrBotApi.Orchestrators;

namespace ContainrBotApi;

public static class PreRequestMethods
{
	public static async ValueTask<object?> CanConnectToOrchestrator(
		EndpointFilterInvocationContext invocationContext,
		EndpointFilterDelegate next)
	{
		var orchestrator = invocationContext.GetArgument<IOrchestrator>(0);
		bool canConnect;
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
		var container = invocationContext.GetArgument<string>(1);

		if (!containers.Exists(e => e.FriendlyName == container))
		{
			return Results.BadRequest(
				$"The container '{container}' does not exist in your 'CONTAINER_LIST' environment variable. Please use the API /debug endpoint for more information");
		}

		return await next(invocationContext);
	}

	public static async ValueTask<object?> IsValidContainer(
		EndpointFilterInvocationContext invocationContext,
		EndpointFilterDelegate next,
		List<Container> containers)
	{
		var orchestrator = invocationContext.GetArgument<IOrchestrator>(0);
		var currentContainer = invocationContext.GetArgument<string>(1);
		var isValidContainer =
			await orchestrator.IsContainerValid(containers.FirstOrDefault(e => e.FriendlyName == currentContainer));

		if (!isValidContainer)
		{
			return Results.BadRequest(
				$"The container '{currentContainer}' is misconfigured in the 'CONTAINER_LIST' environment variable for the selected orchestrator '{orchestrator.Name}'. Required container properties are: {string.Join(", ", orchestrator.RequiredContainerProperties)}. Please use the API /debug endpoint for more information");
		}

		return await next(invocationContext);
	}
}