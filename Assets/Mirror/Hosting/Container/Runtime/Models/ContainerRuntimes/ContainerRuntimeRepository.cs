using System.Threading.Tasks;

namespace Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes
{
    public class ContainerRuntimeRepository
    {
        private readonly ContainerRuntimeClient client;
        public ContainerRuntimeRepository(ContainerRuntimeClient client)
        {
            if (client == null) throw new System.ArgumentNullException(nameof(client));
            this.client = client;
        }
        public Task<string> RuntimeVersion()
        {
            return client.RuntimeVersion();
        }
    }
}
