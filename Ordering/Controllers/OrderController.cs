using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus;
using Microsoft.AspNetCore.Mvc;
using Ordering.AvroSchema;
using Ordering.Models;
using Ordering.Repository;

namespace Ordering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderRepository _orderRepository;
        private readonly IEventBus _eventBus;
        public OrderController(OrderRepository orderRepository, IEventBus eventBus)
        {
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this._eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get() => Ok(_orderRepository.GetOrders());

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id) => Ok(_orderRepository.GetOrder(id));

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] IEnumerable<OrderLineItem> orderLineItems)
        {
            var order = new Order();
            order.OrderStatus = "Created";
            foreach (var item in orderLineItems)
            {
                order.AddOrderLineItems(item);
            }

            _orderRepository.CreateOrder(order);
            var orderResult = _orderRepository.GetOrder(order.Id);


            // generate event data for publish to event bus
            var @event = new OrderCreatedIntegrationEvent()
            {
                OrderId = order.Id
            };

            @event.Products = new List<AvroSchema.Product>();
            foreach(var item in orderResult.OrderLineItems)
            {
                @event.Products.Add(new AvroSchema.Product()
                {
                    ProductName = item.Product.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.UnitPrice
                });
            }

            // public order created event to event bus
            _eventBus.PublishAsync(@event).Wait();

            return Ok(orderResult);
        }
        

    }
}
