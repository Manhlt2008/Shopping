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
    
    public partial class Discount
    {
        public long Id { get; set; }
        public Nullable<long> SaleEventId { get; set; }
        public Nullable<long> ProductId { get; set; }
        public Nullable<double> DiscountPercent { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual SaleEvent SaleEvent { get; set; }
    }
}
