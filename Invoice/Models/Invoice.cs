using System.Collections.Generic;

using System.Linq;

namespace Invoicing.Models
{

    public class Invoice
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public IEnumerable<ProductItem> ProductsItems { get; set; }
        public double InvoicePrice => ProductsItems.Sum(p => p.TotalPrice);
    }
}
