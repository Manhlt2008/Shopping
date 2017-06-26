using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.Supplier
{
    public class SupplierAccount
    {
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public long AccountId { get; set; }
        public byte AccountType { get; set; }
        public byte Status { get; set; }
    }
}