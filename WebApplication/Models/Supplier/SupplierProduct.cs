using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.Supplier
{
    public class SupplierProduct
    {
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public long CreatedSupplierAccountId { get; set; }
        public long ProductId { get; set; }
        public long SupplierCategoryId { get; set; }
        public byte Status { get; set; }
    }
}