using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Events;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Interfaces;

namespace Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes
{
    public enum ContainerRuntimeStatus
    {
        Unknown,
        Running,
        Stopped,
        Error
    }
    public abstract class ContainerRuntimeClient
    {
        private ContainerRuntimeStatus status = ContainerRuntimeStatus.Unknown;
        private readonly IEventBroker eventBroker;
        public ContainerRuntimeStatus Status
        {
            get => status;
            protected set
            {
                if (status == value)
                    return;
                status = value;
                eventBroker.Publish(new ContainerRuntimeStatusChanged(status));
            }
        }
        protected ContainerRuntimeClient(IEventBroker eventBroker)
        {
            this.eventBroker = eventBroker;
            Status = ContainerRuntimeStatus.Unknown;
        }
        public abstract Task<string> RuntimeVersion();
        public abstract Task<IEnumerable<RunningContainer>> CreateRunningContainer(RunningContainerType runningContainerType);
    }
}
