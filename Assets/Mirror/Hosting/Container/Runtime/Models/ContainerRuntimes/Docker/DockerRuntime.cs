using System.Threading.Tasks;

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
            var versionCommand = new DockerCommand(new DockerCommandConfig { arguments = "--version" });
            var versionResponse = await versionCommand.Execute();
            if (!string.IsNullOrEmpty(versionResponse.Output)) return versionResponse.Output;
            return null;
        }
        public override async Task<string> RuntimeVersion()
        {
            return await DockerVersion();
        }
    }
}
