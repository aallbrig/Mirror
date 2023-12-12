using System;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Events;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Interfaces;
using UnityEngine.UIElements;

namespace Mirror.Hosting.Container.Runtime.Controllers
{
    public class ContainerRuntimeStatusController
    {
        private IEventBroker eventBroker;
        private const string runtimeStatusQuery = "user-info_container-runtime-status";
        readonly string runtimeVersionQuery = "user-info_container-runtime-version";
        readonly Label runtimeStatusLabel;
        readonly Label runtimeVersionLabel;
        readonly ContainerRuntimeService runtimeService;
        public ContainerRuntimeStatusController(VisualElement view, ContainerRuntimeService runtimeService, IEventBroker eventBroker)
        {
            if (runtimeService == null) throw new ArgumentNullException(nameof(runtimeService),"container runtime service cannot be null");
            runtimeStatusLabel = view.Q<Label>(runtimeStatusQuery);
            if (runtimeStatusLabel == null) throw new NullReferenceException($"cannot find label with query '{runtimeStatusQuery}'");
            runtimeVersionLabel = view.Q<Label>(runtimeVersionQuery);
            if (runtimeVersionLabel == null) throw new NullReferenceException($"cannot find label with query '{runtimeVersionQuery}'");
            if (eventBroker == null) throw new ArgumentNullException(nameof(eventBroker),"event broker cannot be null");
            this.eventBroker = eventBroker;
            this.eventBroker.Subscribe<ContainerRuntimeStatusChanged>(SyncRuntimeStatus);

            this.runtimeService = runtimeService;
            SyncRuntimeVersion();
        }
        async void SyncRuntimeVersion()
        {
            string runtimeVersion = await runtimeService.GetRuntimeVersion();
            runtimeVersionLabel.text = $"{runtimeVersion}";
        }
        void SyncRuntimeStatus(ContainerRuntimeStatusChanged statusChangedEvent)
        {
            runtimeStatusLabel.text = $"{statusChangedEvent.Status}";
        }
    }
}
