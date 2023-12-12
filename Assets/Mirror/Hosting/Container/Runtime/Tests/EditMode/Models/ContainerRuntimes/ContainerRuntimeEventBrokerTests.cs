using System.Collections;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;

namespace Mirror.Hosting.Container.Runtime.Tests.EditMode.Models.ContainerRuntimes
{
    public class FakeEvent: ContainerRuntimeEvent {}
    public class ContainerRuntimeEventBrokerTests
    {
        [Test]
        public void ContainerRuntimeEventBroker_ASubscriberReceivesEvent()
        {
            ContainerRuntimeEventBroker sut = new ContainerRuntimeEventBroker();
            bool eventDetected = false;
            sut.Subscribe<FakeEvent>(_ => eventDetected = true);

            sut.Publish(new FakeEvent());

            Assert.IsTrue(eventDetected, "expected event to be published");
        }
    }
}
