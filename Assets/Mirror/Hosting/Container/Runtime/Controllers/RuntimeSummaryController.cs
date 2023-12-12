using System;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Events;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Interfaces;
using UnityEngine.UIElements;

namespace Mirror.Hosting.Container.Runtime.Controllers
{
    public class RuntimeSummaryController
    {
        private readonly IEventBroker eventBroker;
        private const string QueryRuntimeStatus = "user-info_container-runtime-status";
        private const string QueryRuntimeVersion = "user-info_container-runtime-version";
        readonly Label runtimeStatusLabel;
        readonly Label runtimeVersionLabel;
        readonly ContainerRuntimeService runtimeService;
        public RuntimeSummaryController(VisualElement view, ContainerRuntimeService runtimeService, IEventBroker eventBroker)
        {
            if (view == null) throw new ArgumentNullException(nameof(view),"view cannot be null");
            if (runtimeService == null) throw new ArgumentNullException(nameof(runtimeService),"runtime service cannot be null");
            if (eventBroker == null) throw new ArgumentNullException(nameof(eventBroker),"event broker cannot be null");

            runtimeStatusLabel = view.Q<Label>(QueryRuntimeStatus);
            if (runtimeStatusLabel == null) throw new NullReferenceException($"cannot find label with query '{QueryRuntimeStatus}'");
            runtimeVersionLabel = view.Q<Label>(QueryRuntimeVersion);
            if (runtimeVersionLabel == null) throw new NullReferenceException($"cannot find label with query '{QueryRuntimeVersion}'");
            this.runtimeService = runtimeService;
            this.eventBroker = eventBroker;

            this.eventBroker.Subscribe<ContainerRuntimeStatusChanged>(SyncRuntimeStatus);
            SyncRuntimeVersion();
        }
        ~RuntimeSummaryController()
        {
            eventBroker.Unsubscribe<ContainerRuntimeStatusChanged>(SyncRuntimeStatus);
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
