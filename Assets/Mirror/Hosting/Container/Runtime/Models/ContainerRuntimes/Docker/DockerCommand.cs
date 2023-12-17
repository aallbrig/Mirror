using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Docker
{
    public class DockerCommand: ContainerRuntimeCommand
    {
        protected override bool Validate(string sanitizedCommandInput)
        {
            // todo: further develop the ruleset to define a valid docker command
            // "leads with docker" rule is a hard constraint; the design philosophy is to be able to read the real docker command
            // when reading the _docker command_ user input so that the user can easily use the command in their own terminal
            // however, it is a design decision that "docker.exe" should not be included in the command. Perhaps this means the
            // docker CLI interactions is actually using a pseudo docker command that then is translated into the real docker command
            // (including turning the executable into "docker.exe" for windows users)
            bool leadsWithDocker = sanitizedCommandInput.StartsWith("docker");
            return leadsWithDocker;
        }
        protected override string Sanitize(string commandInputRaw)
        {
            // todo: actually sanitize command to prevent command injection
            return commandInputRaw;
        }
        protected override IEnumerable<string> ParseArguments(string commandInput)
        {
            // todo: parse arguments more intelligently
            return commandInput.Replace("docker ", "").Split(" ");
        }
        public override async Task<ContainerRuntimeCommandResponse> Execute()
        {
            if (!IsValidCommand)
                throw new Exception($"container hosting | invalid docker command | {this}");

            string cmdArgumentsCombinedStr = "";
            foreach (string commandArg in commandArguments)
                cmdArgumentsCombinedStr += $"{commandArg} ";

            // todo detect if docker is available in path
            using Process dockerProcess = new Process();
#if UNITY_EDITOR_WIN
            dockerProcess.StartInfo.FileName = "docker.exe";
            dockerProcess.StartInfo.Arguments = cmdArgumentsCombinedStr;
#elif UNITY_EDITOR_OSX
            dockerProcess.StartInfo.FileName = "docker";
            dockerProcess.StartInfo.Arguments = cmdArgumentsCombinedStr;
#elif UNITY_EDITOR_LINUX
            dockerProcess.StartInfo.FileName = "/bin/docker";
            dockerProcess.StartInfo.Arguments = $"-c \"{cmdArgumentsCombinedStr}\"";
#else
            dockerProcess.StartInfo.FileName = "docker";
            dockerProcess.StartInfo.Arguments = cmdArgumentsCombinedStr;
#endif
            dockerProcess.StartInfo.RedirectStandardOutput = true;
            dockerProcess.StartInfo.RedirectStandardError = true;
            dockerProcess.StartInfo.UseShellExecute = false;
            dockerProcess.StartInfo.CreateNoWindow = true;

            dockerProcess.Start();

            string output = await dockerProcess.StandardOutput.ReadToEndAsync();
            string error = await dockerProcess.StandardError.ReadToEndAsync();

            dockerProcess.WaitForExit();

            return new ContainerRuntimeCommandResponse { Output = output.TrimEnd('\r', '\n'), Error = error };
        }
        public DockerCommand() : base("") {}
        public DockerCommand(string commandUserInput) : base(commandUserInput) {}
    }
}
