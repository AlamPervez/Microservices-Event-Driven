using EventBus;
using Invoicing.AvroSchema;
using Invoicing.Models;
using Invoicing.Repository;
using Microsoft.Extensions.Logging;
using Ordering.AvroSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Invoicing.IntegrationEvents.Handlers
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        private readonly InvoiceRepository _invoiceRepository;
        private readonly IEventBus _eventBus;
        private readonly ILogger<OrderCreatedIntegrationEventHandler> _logger;
        public OrderCreatedIntegrationEventHandler(InvoiceRepository invoiceRepository, IEventBus eventBus, ILogger<OrderCreatedIntegrationEventHandler> logger)
        {
            this._invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(invoiceRepository));
            this._eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            this._logger = logger??throw new ArgumentNullException(nameof(logger));
        }
        public async Task Handle(OrderCreatedIntegrationEvent @event)
        {
            _logger.LogDebug($"Handling the `Order Created` event from Ordering service");
            var invoice = new Invoice();
            invoice.OrderId = @event.OrderId;

            var productItems = new List<ProductItem>();
            foreach (var item in @event.Products)
            {
                productItems.Add(new ProductItem { ProductName = item.ProductName, Quantity = item.Quantity, UnitPrice = item.UnitPrice });
            }
            invoice.ProductsItems = productItems;

            // both saving to repo and publish to event bus should be an atomic transaction.
            // code is only for POC, not fit for production
            // save the invoice to repository
            await _invoiceRepository.CreateInvoice(invoice);
            
            //build the integration event  
            var invoiceProcessedEvent = new InvoiceProcessedIntegrationEvent()
            {
                InvoiceId = invoice.Id,
                OrderId = invoice.OrderId,
                InvoiceAmount = invoice.InvoicePrice
            };
            //publish the event to event bus
            await _eventBus.PublishAsync(invoiceProcessedEvent);
        }
    }
}
