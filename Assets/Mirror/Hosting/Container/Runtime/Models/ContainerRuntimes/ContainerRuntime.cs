using System;
using System.Threading.Tasks;

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
    public abstract class ContainerRuntime
    {
        public Action<StatusChangedEvent> runtimeStatusChanged;

        public ContainerRuntimeStatus Status { get; private set; }

        protected ContainerRuntime() {
            Status = ContainerRuntimeStatus.Unknown;
        }

        protected void UpdateRuntimeStatus(ContainerRuntimeStatus status)
        {
            ContainerRuntimeStatus oldStatus = Status;
            Status = status;
            runtimeStatusChanged?.Invoke(new StatusChangedEvent { OldStatus = oldStatus, NewStatus = status });
        }
        public abstract Task<string> RuntimeVersion();
    }
}
