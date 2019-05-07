using Dispatch.AvroSchema;
using EventBus;
using Ordering.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.IntegrationEvents.Handlers
{


    public class OrderDispatchIntegrationEventHandler :
        IIntegrationEventHandler<OrderDispatchedIntegrationEvent>

    {
        private readonly OrderRepository _orderRepository;
        public OrderDispatchIntegrationEventHandler(OrderRepository orderRepository)
        {
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }


        public async Task Handle(OrderDispatchedIntegrationEvent @event)
        {
            var order = _orderRepository.GetOrder(@event.OrderId);

            order.DispatchId = @event.DispatchOrderId;
            if (@event.IsDispatched)
            {
                order.OrderStatus = "Dispatched";
            }
            _orderRepository.Save();

            await Task.FromResult(false);
        }
    }
}
