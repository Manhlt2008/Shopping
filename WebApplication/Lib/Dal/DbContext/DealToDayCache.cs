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
    
    public partial class DealToDayCache
    {
        public long Id { get; set; }
        public int DealId { get; set; }
        public string DealName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Address { get; set; }
        public int Quantity { get; set; }
        public int OriginalPrice { get; set; }
        public int Price { get; set; }
        public string StartDate { get; set; }
        public string ExpiredDate { get; set; }
        public string Condition { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
        public string LstOtherImage { get; set; }
        public long ProductId { get; set; }
    
        public virtual Product Product { get; set; }
    }
}
