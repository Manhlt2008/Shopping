using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.APIModel.Delivery
{
    public class DeliveryTransactionOrderModel
    {
        public long Id { get; set; }
        public long DeliveryTransactionId { get; set; }
        public long SupplierId { get; set; }
        public string ExtraInfo { get; set; }
        public string Response { get; set; }
        public long DeliverySystemId { get; set; }
        public long OrderId { get; set; }
        public int Status { get; set; }
        public string Request { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierProvince { get; set; }
        public string SupplierDistrict { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerProvince { get; set; }
        public string CustomerDistrict { get; set; }
        public double Price { get; set; }
        public List<DeliveryTransactionOrderDetailModel> DeliveryTransactionOrderDetailModels { get; set; }
    }
}