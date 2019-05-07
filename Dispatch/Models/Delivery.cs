namespace Dispatch.Models
{

    public class Delivery
    {
        public int DispatchOrderId { get; set; }
        public int InvoiceId { get; set; }
        public double InvoiceAmount { get; set; }
        public int FreightForwarderId { get; set; }
        public string DeliveryAddress { get; set; }
    }

}
