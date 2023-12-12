using System;

namespace Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes
{
    public class ContainerRuntimeCommandResponse
    {
        public Guid CommandId { get; } = Guid.NewGuid();
        public override bool Equals(object obj)
        {
            if (obj == null || !(this.GetType() == obj.GetType()))
                return false;
            ContainerRuntimeCommandResponse eventObj = (ContainerRuntimeCommandResponse)obj;
            return CommandId.Equals(eventObj.CommandId);
        }
        public override int GetHashCode()
        {
            return CommandId.GetHashCode();
        }
        public string Output { get; set; }
        public string Error { get; set; }
    }
}
