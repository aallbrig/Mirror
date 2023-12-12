using System;

namespace Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes
{
    public class ContainerRuntimeEvent
    {
        public Guid EventId { get; private set; } = Guid.NewGuid();
        public object UserData { get; set; }
    }
}
