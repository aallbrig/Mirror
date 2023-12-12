using System;

namespace Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Interfaces
{
    public interface IEventBroker
    {
        void Subscribe<TEvent>(Action<TEvent> subscriber) where TEvent : ContainerRuntimeEvent;
        void Unsubscribe<TEvent>(Action<TEvent> subscriber) where TEvent : ContainerRuntimeEvent;
        void Publish<TEvent>(TEvent @event) where TEvent : ContainerRuntimeEvent;
    }
}
