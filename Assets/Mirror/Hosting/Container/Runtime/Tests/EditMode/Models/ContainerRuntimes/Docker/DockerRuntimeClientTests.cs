using System.Collections.Generic;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Docker;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Events;
using Mirror.Hosting.Container.Runtime.Models.Enums;
using NUnit.Framework;

namespace Mirror.Hosting.Container.Runtime.Tests.EditMode.Models.ContainerRuntimes.Docker
{
    public class DockerRuntimeClientTests
    {
        [Test]
        public async void DockerRuntime_AbleToObtainDockerVersion()
        {
            DockerRuntimeClient sut = new DockerRuntimeClient(new ContainerRuntimeEventBroker());

            string versionOutput = await sut.RuntimeVersion();

            Assert.NotNull(versionOutput);
        }
        [Test]
        public async void DockerRuntime_AbleToQueryRunningGameClientWebGlContainers()
        {
            DockerRuntimeClient sut = new DockerRuntimeClient(new ContainerRuntimeEventBroker());
            IEnumerable<RunningContainer> runningContainer = await sut.CreateRunningContainer(RunningContainerType.GameClientWebGL);
            Assert.NotNull(runningContainer, "expected running container to be created");

            IEnumerable<RunningContainer> runningContainers = await sut.RunningContainers(RunningContainerType.GameClientWebGL);
            Assert.NotNull(runningContainers, "expected to find running containers");
        }
        [Test]
        public async void DockerRuntime_PublishesEventOnRuntimeStatusChanged()
        {
            ContainerRuntimeEventBroker containerRuntimeEventBroker = new ContainerRuntimeEventBroker();
            DockerRuntimeClient sut = new DockerRuntimeClient(containerRuntimeEventBroker);
            bool statusChangeEventDetected = false;
            containerRuntimeEventBroker.Subscribe<ContainerRuntimeStatusChanged>(_ => statusChangeEventDetected = true);

            await sut.RuntimeVersion();

            Assert.IsTrue(statusChangeEventDetected, "expected status change event to be published");
        }
    }
}
