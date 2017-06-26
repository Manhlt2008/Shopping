using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication.Models.User;

namespace WebApplication.Models.HomPage
{
    public class TypeHomePageModel : ResultModel
    {
        [Display(Name = "Id")]
        public long Id { get; set; }
        [Display(Name = "TypeCode")]
        public string TypeCode { get; set; }
        [Display(Name = "TypeName")]
        public string TypeName { get; set; }
        [Display(Name = "Status")]
        public int Status { get; set; }
    }
}