using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.APIModel.Delivery
{
    public class DeliveryTransactionOrderDetailModel
    {
        public long Id { get; set; }
        public long DelviveryTransactionOrderId { get; set; }
        public long OrderDetailId { get; set; }
        public int Status { get; set; }
        public string Response { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }

    }
}