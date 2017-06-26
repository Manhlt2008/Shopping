using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.Order
{
    public class FindAllByUserNameOrOrderCodeRequestModel
    {
        public string QueryString { get; set; }

        public List<int> FilterOptions { get; set; }
    }
}