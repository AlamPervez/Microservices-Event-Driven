using Dispatch.Models;
using Dispatch.Repository;
using EventBus;
using Microsoft.Extensions.Logging;
using Ordering.AvroSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dispatch.IntegrationEvents.Handlers
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        public readonly DispatchRepository _dispatchRepository;
        private readonly ILogger<OrderCreatedIntegrationEventHandler> _logger;
        public OrderCreatedIntegrationEventHandler(DispatchRepository dispatchRepository,
             ILogger<OrderCreatedIntegrationEventHandler> logger)
        {
            this._dispatchRepository = dispatchRepository??throw new ArgumentNullException(nameof(dispatchRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task Handle(OrderCreatedIntegrationEvent @event)
        {
            _logger.LogDebug($"Handling the `Order Created` event from Order service");
            var dispatchOrder = new DispatchOrder()
            {
                OrderId = @event.OrderId,
            };

            // set the status to not dispatched yet
            dispatchOrder.DisptachStatus = "Not Dispatched";
                      
            //save to database
            _dispatchRepository.CreateDispatchOrder(dispatchOrder);
          await  Task.FromResult(false);
        }
    }
}
