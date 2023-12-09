using System;
using System.Runtime.InteropServices;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes;
using UnityEngine.UIElements;

namespace Mirror.Hosting.Container.Runtime.Controllers
{
    public class ContainerRuntimeStatusController
    {
        private readonly string runtimeStatusQuery = "user-info_container-runtime-status";
        private readonly string runtimeVersionQuery = "user-info_container-runtime-version";
        private readonly Label runtimeStatusLabel;
        private readonly Label runtimeVersionLabel;
        private readonly ContainerRuntime runtime;
        public ContainerRuntimeStatusController(VisualElement view, ContainerRuntime runtime)
        {
            runtimeStatusLabel = view.Q<Label>(runtimeStatusQuery);
            if (runtimeStatusLabel == null) throw new NullReferenceException($"cannot find label with query '{runtimeStatusQuery}'");
            runtimeVersionLabel = view.Q<Label>(runtimeVersionQuery);
            if (runtimeVersionLabel == null) throw new NullReferenceException($"cannot find label with query '{runtimeVersionQuery}'");
            if (runtime == null) throw new NullReferenceException("container runtime cannot be null");

            this.runtime = runtime;
            this.runtime.runtimeStatusChanged += SyncRuntimeStatus;
            runtimeStatusLabel.text = $"Container Runtime Status: {runtime.Status}";
            SyncRuntimeVersion();
        }
        private async void SyncRuntimeVersion()
        {
            string runtimeVersion = await runtime.RuntimeVersion();
            runtimeVersionLabel.text = $"{runtimeVersion}";
        }
        private void SyncRuntimeStatus(StatusChangedEvent statusChangedEvent)
        {
            runtimeStatusLabel.text = $"{statusChangedEvent.NewStatus}";
        }
    }
}
