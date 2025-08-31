using System.Diagnostics;

namespace GameServerApi.Services;

public class DockerService : IDockerService
{
    public string GetContainerId(string containerName)
    {
        string[] result = GetAllContainers();

        List<string> containers = result.Where(l => l.Contains(containerName)).ToList();

        return containers.Select(l => l[..12]).ToList().FirstOrDefault() ?? "";
    }

    private static string[] GetAllContainers()
    {
        Process p = new()
        {
            StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = "docker",
                Arguments = $"ps -a"
            }
        };

        p.Start();

        string[] result = p.StandardOutput.ReadToEnd().Split('\n');
        p.WaitForExit();

        return result;
    }

    public bool StartContainer(string id)
    {
        Process p = new()
        {
            StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = "docker",
                Arguments = $"container start {id}"
            }
        };

        p.Start();

        p.WaitForExit();

        return p.ExitCode == 0;
    }

    public bool StopContainer(string id)
    {
        Process p = new()
        {
            StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = "docker",
                Arguments = $"container stop {id}"
            }
        };

        p.Start();

        p.WaitForExit();

        return p.ExitCode == 0;
    }

    public bool IsContainerRunning(string containerName)
    {
        Process p = new()
        {
            StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = "docker",
                Arguments = $"container inspect -f \"{{{{.State.Running}}}}\" {containerName}"
            }
        };

        p.Start();

        string result = p.StandardOutput.ReadToEnd();
        p.WaitForExit();

        return result.Trim('\n') == "true";
    }
}
