using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.Models.GHTK
{
    public class ExistsOrder
    {
        [JsonProperty("order_id")]
        public string OrderId { get; set; }
        [JsonProperty("approved_date")]
        public DateTime ApprovedDate { get; set; }
        [JsonProperty("fee")]
        public double Fee { get; set; }

    }
}