using System.Collections.Generic;
using System.Threading.Tasks;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Interfaces;

namespace Mirror.Hosting.Container.Runtime.Tests.EditMode.Fakes
{
    public class FakeRuntimeClient: ContainerRuntimeClient
    {

        public FakeRuntimeClient(IEventBroker eventBroker) : base(eventBroker) {}
        public override Task<string> RuntimeVersion()
        {
            return Task.FromResult("fake runtime version");
        }
        public override Task<IEnumerable<RunningContainer>> CreateRunningContainer(RunningContainerType runningContainerType)
        {
            return Task.FromResult(new List<RunningContainer> { new RunningContainer()} as IEnumerable<RunningContainer>);
        }
    }
}
