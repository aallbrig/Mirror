using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Docker;
using NUnit.Framework;

namespace Mirror.Hosting.Container.Runtime.Tests.EditMode
{
    public class DockerRuntimeTests
    {
        [Test]
        public async void DockerRuntime_AbleToObtainDockerVersion()
        {
            DockerRuntime sut = new DockerRuntime();

            var versionOutput = await sut.DockerVersion();

            Assert.NotNull(versionOutput);
        }
        [Test]
        public async void DockerRuntime_AbleToGetRunningWebGLDockerContainers()
        {
            DockerRuntime sut = new DockerRuntime();

            var runningContainers = await sut.RunningContainers(RunningContainerType.GameClient_WebGL);

            Assert.NotNull(runningContainers);
        }
    }
}
