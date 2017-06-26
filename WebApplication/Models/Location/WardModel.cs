using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models.Location
{
    public class WardModel
    {
        [Display(Name = "WardId")]
        public string WardId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "DistrictId")]
        public string DistrictId { get; set; }
    }
}