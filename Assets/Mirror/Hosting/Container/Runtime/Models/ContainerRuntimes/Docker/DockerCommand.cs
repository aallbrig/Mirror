using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Docker
{
    public class DockerCommand
    {
        static bool ValidateCommand(string dockerCommandInput)
        {
            // todo: further develop the ruleset to define a valid docker command
            // "leads with docker" rule is a hard constraint; the design philosophy is to be able to read the real docker command
            // when reading the _docker command_ user input so that the user can easily use the command in their own terminal
            // however, it is a design decision that "docker.exe" should not be included in the command. Perhaps this means the
            // docker CLI interactions is actually using a pseudo docker command that then is translated into the real docker command
            // (including turning the executable into "docker.exe" for windows users)
            var leadsWithDocker = dockerCommandInput.StartsWith("docker");
            return leadsWithDocker;
        }
        static string SanitizeCommand(string dockerCommandInput)
        {
            // todo: sanitize command to prevent command injection
            return dockerCommandInput;
        }

        public bool IsValidCommand { get; private set; }
        private readonly string dockerCommandInput;
        private readonly IEnumerable<string> dockerCommandArguments;
        public override string ToString()
        {
            return dockerCommandInput;
        }
        public DockerCommand(string dockerCommandInput)
        {
            this.dockerCommandInput = SanitizeCommand(dockerCommandInput);
            IsValidCommand = ValidateCommand(this.dockerCommandInput);
            if (!IsValidCommand)
            {
                Debug.Log($"Container Hosting | Invalid docker command | {dockerCommandInput}");
                return;
            }
            dockerCommandArguments = ParseArguments(this.dockerCommandInput);
        }
        static IEnumerable<string> ParseArguments(string dockerCommandInput)
        {
            return dockerCommandInput.Replace("docker", "").Split(" ");
        }
        public async Task<DockerCommandResponse> Execute()
        {
            if (!IsValidCommand)
            {
                throw new Exception("Invalid docker command");
            }

            var dockerCommandArgs = "";
            foreach (string dockerCommandArgument in dockerCommandArguments)
                dockerCommandArgs += $"{dockerCommandArgument} ";
            try
            {
                using Process dockerProcess = new Process();
                // TODO: add support for windows, linux
                dockerProcess.StartInfo.FileName = "docker";
                dockerProcess.StartInfo.Arguments = dockerCommandArgs;
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
                Debug.Log($"Container Hosting | Failed to execute docker command '{this}' | {ex}");
            }
            return null;
        }
    }
}
