using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ContainrBot.Library;

public static class Helpers
{
	public static string GetRequiredEnvironmentVariable(IHostApplicationBuilder builder, string environmentVariable)
	{
		var variableValue = builder.Configuration.GetValue<string>(environmentVariable);

		return !string.IsNullOrEmpty(variableValue)
			? variableValue
			: throw new InvalidOperationException($"Environment variable not set: {environmentVariable}");
	}
}