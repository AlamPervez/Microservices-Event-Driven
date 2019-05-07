namespace Dispatch.Models
{

    public class DispatchOrder
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Delivery Delivery { get; set; }
        public string DisptachStatus { get; set; }
    }

}
