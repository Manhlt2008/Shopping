using System.Collections.Generic;

namespace WebApplication.Models.Order
{
    public class UpdateOrderModelRequest
    {
        public long OrderId { get; set; }
        public int Status { get; set; }
        public long UserId { get; set; }
        public List<UpdateOrderDetailModelRequest> OrderDetails { get; set; }
    }
}