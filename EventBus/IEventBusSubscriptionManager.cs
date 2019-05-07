using System;
using System.Collections.Generic;

namespace EventBus
{
    public interface IEventBusSubscriptionManager
    {
        bool IsEmpty { get; }

        void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        bool HasEvent<T>() where T : IntegrationEvent;


        void Clear();

        IEnumerable<Type> GetHandlersForEvent<T>() where T : IntegrationEvent;

    }


}


