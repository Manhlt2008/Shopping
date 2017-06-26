using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models.Order
{
    public class OrderModel
    {
        [Display(Name = "Id")]
        public long Id { get; set; }
        [Display(Name = "UserId")]
        public long UserId { get; set; }
        [Display(Name = "TotalPrice")]
        public double TotalPrice { get; set; }
        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "CreatedDateStr")]
        public string CreatedDateStr { get; set; }
        [Display(Name = "Status")]
        public int Status { get; set; }
        [Display(Name = "Code")]
        public string Code { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Display(Name = "Province")]
        public string Province { get; set; }
        [Display(Name = "District")]
        public string District { get; set; }

    }
}