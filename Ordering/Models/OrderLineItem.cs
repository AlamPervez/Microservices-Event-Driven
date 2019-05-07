using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Models
{

    public class OrderLineItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        
        public int Quantity { get; set; }

        public double TotalPrice => this.Product.UnitPrice * this.Quantity;

        
    }
}
