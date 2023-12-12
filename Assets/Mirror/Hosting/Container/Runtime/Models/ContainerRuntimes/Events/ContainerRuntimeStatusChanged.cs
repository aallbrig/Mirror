namespace Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Events
{
    public class ContainerRuntimeStatusChanged : ContainerRuntimeEvent
    {
        public ContainerRuntimeStatusChanged(ContainerRuntimeStatus status)
        {
            Status = status;
        }
        public ContainerRuntimeStatus Status { get; }
    }
}
