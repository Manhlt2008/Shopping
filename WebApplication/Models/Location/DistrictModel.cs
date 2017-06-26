using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models.Location
{
    public class DistrictModel
    {
        [Display(Name = "DistrictId")]
        public string DistrictId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "ProvinceId")]
        public string ProvinceId { get; set; }

    }
}