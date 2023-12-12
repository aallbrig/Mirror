using System;

namespace Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes
{
    public class ContainerRuntimeEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public override bool Equals(object obj)
        {
            if (obj == null || !(this.GetType() == obj.GetType()))
                return false;
            else
            {
                ContainerRuntimeEvent eventObj = (ContainerRuntimeEvent)obj;
                return EventId.Equals(eventObj.EventId);
            }
        }
        public override int GetHashCode()
        {
            return EventId.GetHashCode();
        }
    }
}
