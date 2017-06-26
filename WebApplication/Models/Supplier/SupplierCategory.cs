using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.Supplier
{
    public class SupplierCategory
    {
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public long CategoryId { get; set; }
        public byte Status { get; set; }
    }
}