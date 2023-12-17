using System;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Interfaces;
using UnityEngine;

namespace Mirror.Hosting.Container.Common.Tests.Fakes
{
    public class FakeEventBroker: IEventBroker
    {
        public void Subscribe<TEvent>(Action<TEvent> subscriber) where TEvent : ContainerRuntimeEvent
        {
            Debug.Log("FakeEventBroker.Subscribe");
        }
        public void Unsubscribe<TEvent>(Action<TEvent> subscriber) where TEvent : ContainerRuntimeEvent
        {
            Debug.Log("FakeEventBroker.Unsubscribe");
        }
        public void Publish<TEvent>(TEvent @event) where TEvent : ContainerRuntimeEvent
        {
            Debug.Log("FakeEventBroker.Publish");
        }
    }
}
