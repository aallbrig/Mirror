using System.Threading.Tasks;

namespace Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes
{
    public class ContainerRuntimeService
    {
        readonly ContainerRuntimeRepository repository;
        public ContainerRuntimeService(ContainerRuntimeRepository repository)
        {
            if (repository == null) throw new System.ArgumentNullException(nameof(repository));

            this.repository = repository;
        }
        public Task<string> GetRuntimeVersion()
        {
            return repository.RuntimeVersion();
        }
    }
}
