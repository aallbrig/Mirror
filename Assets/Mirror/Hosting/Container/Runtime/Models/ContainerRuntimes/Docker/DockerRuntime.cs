using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Docker
{

    public class DockerRuntime: ContainerRuntime
    {
        private ContainerRuntimeStatus runtimeStatus;
        public DockerRuntime()
        {
            UpdateRuntimeStatus(ContainerRuntimeStatus.Unknown);
            QueryRuntimeStatus();
        }
        private async void QueryRuntimeStatus()
        {
            string dockerInstalled = await DockerVersion();
            UpdateRuntimeStatus(dockerInstalled != null ? ContainerRuntimeStatus.Running : ContainerRuntimeStatus.Error);
        }
        public async Task<string> DockerVersion()
        {
            DockerCommand versionCommand = new DockerCommand("docker --version");
            if (!versionCommand.IsValidCommand) return null;
            DockerCommandResponse versionResponse = await versionCommand.Execute();
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

            DockerCommandResponse runningContainersResponse = await runningContainersCommand.Execute();
            if (!string.IsNullOrEmpty(runningContainersResponse.Error)) {
                Debug.LogError($"container hosting | docker runtime | command error for command '{this}'\noutput\n'{runningContainersResponse.Error}'");
                return null;
            }
            if (string.IsNullOrEmpty(runningContainersResponse.Output)) {
                Debug.LogError($"container hosting | docker runtime | empty or null output '{runningContainersResponse.Output}'");
                return null;
            }
            IEnumerable<RunningContainer> runningContainers = ParseRunningContainers(runningContainersResponse.Output);
            IEnumerable<RunningContainer> runningContainerList = runningContainers.ToList();
            if (!runningContainerList.Any()) {
                Debug.LogError($"container hosting | docker runtime | no running containers found for type '{runningContainerQuery}'");
                return null;
            }

            return runningContainerList;
        }
        private static IEnumerable<RunningContainer> ParseRunningContainers(string dockerPsCommandOutput)
        {
            List<RunningContainer> runningContainers = new List<RunningContainer>();
            string[] dockerPsCommandOutputLines = dockerPsCommandOutput.Split('\n');
            foreach (string maybeRunningContainerRow in dockerPsCommandOutputLines)
            {
                RunningContainer maybeRunningContainerParse = ParseRunningContainer(maybeRunningContainerRow);
                if (maybeRunningContainerRow == null) continue;

                runningContainers.Add(maybeRunningContainerParse);
            }
            return runningContainers;
        }
        private static RunningContainer ParseRunningContainer(string maybeRunningContainerRow)
        {
            return RunningContainer.Parse(maybeRunningContainerRow);
        }
    }
}
