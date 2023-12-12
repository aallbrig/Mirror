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
    public struct StatusChangedEvent
    {
        public ContainerRuntimeStatus NewStatus { get; set; }
        public ContainerRuntimeStatus OldStatus { get; set; }
    }
    public abstract class ContainerRuntimeClient
    {
        public Action<StatusChangedEvent> runtimeStatusChanged;

        private ContainerRuntimeStatus _status = ContainerRuntimeStatus.Unknown;
        private readonly IEventBroker eventBroker;

        public ContainerRuntimeStatus Status
        {
            get => _status;
            protected set
            {
                if (_status != value)
                {
                    _status = value;
                    eventBroker.Publish(new ContainerRuntimeStatusChanged(_status));
                }
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
