namespace Invoicing.Models
{
    public class ProductItem
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice => this.Quantity * this.UnitPrice;

    }
}
