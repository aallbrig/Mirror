using System;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes;
using NUnit.Framework;

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
        [Test]
        public void ContainerRuntimeEventBroker_CanUnsubscribe()
        {
            ContainerRuntimeEventBroker sut = new ContainerRuntimeEventBroker();
            bool firstEventDetected = false;
            bool secondEventDetected = false;
            Action<FakeEvent> subscription = _ =>
            {
                if (!firstEventDetected)
                {
                    firstEventDetected = true;
                    return;
                }
                secondEventDetected = true;
            };
            sut.Subscribe(subscription);
            sut.Publish(new FakeEvent());
            Assert.IsTrue(firstEventDetected);
            Assert.IsFalse(secondEventDetected);
            sut.Unsubscribe(subscription);

            sut.Publish(new FakeEvent());

            Assert.IsFalse(secondEventDetected, "second event should not have been detected");
        }
        [Test]
        public void ContainerRuntimeEventBroker_DoesNotAllowMultipleOfSameSubscription()
        {
            ContainerRuntimeEventBroker sut = new ContainerRuntimeEventBroker();
            int callCount = 0;
            Action<FakeEvent> subscription = _ =>
            {
                callCount++;
            };
            sut.Subscribe(subscription);
            sut.Subscribe(subscription);
            sut.Subscribe(subscription);

            sut.Publish(new FakeEvent());

            Assert.AreEqual(1, callCount, "same subscription should not be called multiple times");
        }
        [Test]
        public void ContainerRuntimeEventBroker_AllowsMultipleSubscriptions()
        {
            ContainerRuntimeEventBroker sut = new ContainerRuntimeEventBroker();
            bool eventDetected = false;
            int callCount = 0;
            Action<FakeEvent> subscriptionA = _ =>
            {
                eventDetected = true;
            };
            Action<FakeEvent> subscriptionB = _ =>
            {
                callCount++;
            };
            sut.Subscribe(subscriptionA);
            sut.Subscribe(subscriptionB);

            sut.Publish(new FakeEvent());

            Assert.IsTrue(eventDetected, "both subscription A and B should be called (A not called)");
            Assert.AreEqual(1, callCount, "both subscription A and B should be called (B not called)");
        }
    }
}
