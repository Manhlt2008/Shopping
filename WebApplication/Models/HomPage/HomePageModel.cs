using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication.Models.User;

namespace WebApplication.Models.HomPage
{
    public class HomePageModel : ResultModel
    {
        [Display(Name = "Id")]
        public long Id { get; set; }
        [Display(Name = "ProductId")]
        public long ProductId { get; set; }
        [Display(Name = "ProductName")]
        public string ProductName { get; set; }
        [Display(Name = "StartDate")]
        public DateTime StartDate { get; set; }
        [Display(Name = "EndDate")]
        public DateTime EndDate { get; set; }
        [Display(Name = "IOrder")]
        public int IOrder { get; set; }
        [Display(Name = "TypeHomePageId")]
        public long TypeHomePageId { get; set; }
        [Display(Name = "TypeHomePageName")]
        public string TypeHomePageName { get; set; }
        [Display(Name = "Status")]
        public int Status { get; set; }
    }
}