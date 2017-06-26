using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication.Models.User;

namespace WebApplication.Models.Transaction
{
    public class TransactionModel : ResultModel
    {
        [Display(Name = "Id")]
        public long Id { get; set; }
        [Display(Name = "OrderId")]
        public long OrderId { get; set; }
        [Display(Name = "OrderCode")]
        public string OrderCode { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "PaymentType")]
        public int PaymentType { get; set; }
        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "TransferAmount")]
        public double TransferAmount { get; set; }
        [Display(Name = "TotalPrice")]
        public double TotalPrice { get; set; }
        [Display(Name = "ExtraInfo")]
        public string ExtraInfo { get; set; }
        [Display(Name = "Status")]
        public int Status { get; set; }
    }
}