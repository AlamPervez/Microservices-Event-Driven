using Dispatch.AvroSchema;
using Dispatch.Models;
using Dispatch.Repository;
using EventBus;
using Invoicing.AvroSchema;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dispatch.IntegrationEvents.Handlers
{
    public class InvoiceProcessedIntegrationEventHandler : IIntegrationEventHandler<InvoiceProcessedIntegrationEvent>
    {
        private readonly DispatchRepository _dispatchOrderRepository;
        private readonly IEventBus _eventBus;
        private readonly ILogger<InvoiceProcessedIntegrationEventHandler> _logger;
        public InvoiceProcessedIntegrationEventHandler(DispatchRepository dispatchOrderRepository, IEventBus eventBus
            , ILogger<InvoiceProcessedIntegrationEventHandler> logger)
        {
            this._dispatchOrderRepository = dispatchOrderRepository ?? throw new ArgumentNullException(nameof(dispatchOrderRepository));
            this._eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task Handle(InvoiceProcessedIntegrationEvent @event)
        {
            
            _logger.LogDebug($"Handling the `Invoice Processed` event from Invoice service");
            // get the dispatched order from repository
            var dispatchOrder = _dispatchOrderRepository.GetDispatchOrderByOrderId(@event.OrderId);

            var delivery = new Delivery()
            {
                DeliveryAddress = "John Doe, Neverland, Zip - 000000",
                FreightForwarderId = new Random().Next(111111, 999999),
                InvoiceAmount = @event.InvoiceAmount
            };
            dispatchOrder.Delivery = delivery;
            dispatchOrder.DisptachStatus = "Dispatched";
            // save to repository
            _dispatchOrderRepository.Save();

            // build the event for Order dispatched
            var orderDispatchedIntegrationEvent = new OrderDispatchedIntegrationEvent()
            {
                DispatchOrderId = dispatchOrder.Id,
                OrderId = dispatchOrder.OrderId,
                IsDispatched = true
            };

            //publish the event
            await _eventBus.PublishAsync(orderDispatchedIntegrationEvent);
        }
    }
}
