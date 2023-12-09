using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Docker
{
    public class DockerCommand
    {
        readonly DockerCommandConfig _config;
        public DockerCommand(DockerCommandConfig config)
        {
            _config = config;
        }
        public async Task<DockerCommandResponse> Execute()
        {
            try
            {
                // TODO: add support for windows, linux
                using Process dockerProcess = new Process();
                dockerProcess.StartInfo.FileName = "docker";
                dockerProcess.StartInfo.Arguments = _config.arguments;
                dockerProcess.StartInfo.RedirectStandardOutput = true;
                dockerProcess.StartInfo.RedirectStandardError = true;
                dockerProcess.StartInfo.UseShellExecute = false;
                dockerProcess.StartInfo.CreateNoWindow = true;

                dockerProcess.Start();

                string output = await dockerProcess.StandardOutput.ReadToEndAsync();
                string error = await dockerProcess.StandardError.ReadToEndAsync();

                dockerProcess.WaitForExit();

                return new DockerCommandResponse { Output = output.TrimEnd('\r', '\n'), Error = error };
            }
            catch (Exception ex)
            {
                Debug.Log($"Container Hosting | Failed to execute docker command | {ex}");
            }
            return null;
        }
    }
}
