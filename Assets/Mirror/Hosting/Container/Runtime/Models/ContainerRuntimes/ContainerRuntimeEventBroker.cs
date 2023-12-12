using System;
using System.Collections.Generic;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Interfaces;

namespace Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes
{
    public class ContainerRuntimeEventBroker:IEventBroker
    {
        private readonly Dictionary<Type, List<Delegate>> eventSubscriptions = new Dictionary<Type, List<Delegate>>();
        public void Subscribe<TEvent>(Action<TEvent> subscriber) where TEvent : ContainerRuntimeEvent
        {
            Type eventType = typeof(TEvent);
            if (!eventSubscriptions.ContainsKey(eventType))
                eventSubscriptions[eventType] = new List<Delegate>();
            if (!eventSubscriptions[eventType].Contains(subscriber))
                eventSubscriptions[eventType].Add(subscriber);
        }
        public void Unsubscribe<TEvent>(Action<TEvent> subscriber) where TEvent : ContainerRuntimeEvent
        {
            Type eventType = typeof(TEvent);
            eventSubscriptions[eventType]?.Remove(subscriber);
        }
        public void Publish<TEvent>(TEvent eventToPublish) where TEvent : ContainerRuntimeEvent
        {
            Type eventType = eventToPublish.GetType();
            if (!eventSubscriptions.TryGetValue(eventType, value: out List<Delegate> subscribers))
                return;
            foreach (Delegate subscriber in subscribers)
                (subscriber as Action<TEvent>)?.Invoke(eventToPublish);
        }
    }
}
