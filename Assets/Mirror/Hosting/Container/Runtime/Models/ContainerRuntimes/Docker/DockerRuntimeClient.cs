using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Docker
{

    public class DockerRuntimeClient: ContainerRuntimeClient
    {
        private ContainerRuntimeStatus runtimeStatus;
        public DockerRuntimeClient(ContainerRuntimeEventBroker containerRuntimeEventBroker) : base(containerRuntimeEventBroker)
        {
            QueryRuntimeStatus();
        }
        private async void QueryRuntimeStatus()
        {
            string dockerInstalled = await DockerVersion();
            Status = (dockerInstalled != null) ? ContainerRuntimeStatus.Running : ContainerRuntimeStatus.Error;
        }
        private async Task<string> DockerVersion()
        {
            ContainerRuntimeCommand versionCommand = new DockerCommand("docker --version");
            if (!versionCommand.IsValidCommand) return null;
            ContainerRuntimeCommandResponse versionResponse = await versionCommand.Execute();
            if (!string.IsNullOrEmpty(versionResponse.Output)) return versionResponse.Output;
            return null;
        }
        public override async Task<string> RuntimeVersion()
        {
            return await DockerVersion();
        }
        public async Task<IEnumerable<RunningContainer>> RunningContainers(RunningContainerType runningContainerQuery)
        {
            DockerCommand runningContainersCommand;
            switch (runningContainerQuery)
            {
                case RunningContainerType.GameClient_WebGL:
                    runningContainersCommand = new DockerCommand("docker ps --filter \"label=container-hosting_running-container-type=game-client_webgl\"");
                    break;
                default:
                    runningContainersCommand = null;
                    break;
            }
            if (runningContainersCommand == null)
            {
                Debug.LogError($"container hosting | docker runtime | unsupported running container type '{runningContainerQuery}'");
                return null;
            }
            if (!runningContainersCommand.IsValidCommand)
            {
                Debug.LogError($"container hosting | docker runtime | invalid command '{this}'");
                return null;
            }

            ContainerRuntimeCommandResponse runningContainersResponse = await runningContainersCommand.Execute();
            if (!string.IsNullOrEmpty(runningContainersResponse.Error)) {
                Debug.LogError($"container hosting | docker runtime | command error for command '{this}'\noutput\n'{runningContainersResponse.Error}'");
                return null;
            }
            if (string.IsNullOrEmpty(runningContainersResponse.Output)) {
                Debug.LogError($"container hosting | docker runtime | empty or null output '{runningContainersResponse.Output}'");
                return null;
            }
            IEnumerable<RunningContainer> runningContainers = ParseRunningContainers(runningContainersResponse.Output);

            return runningContainers;
        }
        private static IEnumerable<RunningContainer> ParseRunningContainers(string dockerPsCommandOutput)
        {
            string[] dockerPsCommandOutputLines = dockerPsCommandOutput.Split('\n');
            List<RunningContainer> runningContainers = new List<RunningContainer>();
            foreach (string maybeRunningContainerRow in dockerPsCommandOutputLines)
            {
                RunningContainer maybeRunningContainerParse = RunningContainer.Parse(maybeRunningContainerRow);
                if (maybeRunningContainerRow == null) continue;

                runningContainers.Add(maybeRunningContainerParse);
            }
            return runningContainers;
        }
        public override Task<IEnumerable<RunningContainer>> CreateRunningContainer(RunningContainerType gameClientWebGL)
        {
            return null;
        }
    }
}
