using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Models
{

    public class Order
    {
        private readonly List<OrderLineItem> _orderLineItems;

        public Order()
        {
            this._orderLineItems = new List<OrderLineItem>();
        }
        public int Id { get; set; }

        public IReadOnlyCollection<OrderLineItem> OrderLineItems => _orderLineItems;
        public string OrderStatus { get; set; }

        public void AddOrderLineItems(OrderLineItem orderLineItem)
        {
            _orderLineItems.Add(orderLineItem);
        }

        public double TotalPrice=>_orderLineItems.Sum(o=>o.TotalPrice);
        public int? DispatchId { get; set; }
    }
}
