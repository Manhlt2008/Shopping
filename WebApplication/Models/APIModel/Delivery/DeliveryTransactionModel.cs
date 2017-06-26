using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.APIModel.Delivery
{
    public class DeliveryTransactionModel
    {
        public long Id { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string ExtraInfo { get; set; }
        public string Response { get; set; }
        public string Request { get; set; }
        public long CreatedByAccountId { get; set; }

        public List<DeliveryTransactionOrderModel> DeliveryTransactionOrderModels { get; set; }

    }
}