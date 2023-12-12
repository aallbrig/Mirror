namespace Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes
{
    // todo: ensure this model is _generically_ usable for all container runtimes (not just docker)
    public class RunningContainer
    {
        public static RunningContainer Parse(string dockerPsCommandOutputLine)
        {
            // e.g. 7f4160e11661   docker-compose-tanks_reverse_proxy        "/docker-entrypoint.…"   2 weeks ago   Up 2 weeks   80/tcp, 0.0.0.0:9443->443/tcp                    docker-compose-tanks_reverse_proxy-1
            if (string.IsNullOrEmpty(dockerPsCommandOutputLine)) return null;
            var runningContainerRow = dockerPsCommandOutputLine.Trim();
            if (string.IsNullOrEmpty(runningContainerRow)) return null;
            // var runningContainerRowParts = runningContainerRow.Split(' ');
            // if (runningContainerRowParts.Length < 7) return null;

            return new RunningContainer
            {
                // ContainerID = runningContainerRowParts[0],
            };
        }
        public string ContainerID { get; private set; }
        public string Image { get; private set; }
        public string Command { get; private set; }
        public string Created { get; private set; }
        public string Status { get; private set; }
        public string Ports { get; private set; }
        public string Names { get; private set; }
    }
}
