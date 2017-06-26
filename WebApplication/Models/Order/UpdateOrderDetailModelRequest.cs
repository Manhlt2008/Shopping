namespace WebApplication.Models.Order
{
    public class UpdateOrderDetailModelRequest
    {
        public long OrderDetailId { get; set; }

        public long ProductId { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }

        public double Discount { get; set; }
    }
}