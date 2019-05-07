using System;
using System.Threading.Tasks;

namespace EventBus
{ 
    public interface IIntegrationEventHandler<in T>
        where T : IntegrationEvent
    {
        Task Handle(T @event);
    }
}
