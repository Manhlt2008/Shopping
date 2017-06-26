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
    
    public partial class SupplierAccount
    {
        public SupplierAccount()
        {
            this.SupplierProducts = new HashSet<SupplierProduct>();
        }
    
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public long AccountId { get; set; }
        public byte AccountType { get; set; }
        public byte Status { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<SupplierProduct> SupplierProducts { get; set; }
    }
}
