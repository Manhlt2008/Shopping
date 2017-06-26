//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication.Lib.Dal.DbContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class DeliveryTransactionOrder
    {
        public DeliveryTransactionOrder()
        {
            this.DeliveryTransactionOrderDetails = new HashSet<DeliveryTransactionOrderDetail>();
        }
    
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
    
        public virtual DeliveryTransaction DeliveryTransaction { get; set; }
        public virtual Order Order { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<DeliveryTransactionOrderDetail> DeliveryTransactionOrderDetails { get; set; }
    }
}