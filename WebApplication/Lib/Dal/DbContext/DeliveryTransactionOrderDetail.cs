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
    
    public partial class DeliveryTransactionOrderDetail
    {
        public long Id { get; set; }
        public long DelviveryTransactionOrderId { get; set; }
        public long OrderDetailId { get; set; }
        public int Status { get; set; }
        public string Response { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    
        public virtual DeliveryTransactionOrder DeliveryTransactionOrder { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }
    }
}