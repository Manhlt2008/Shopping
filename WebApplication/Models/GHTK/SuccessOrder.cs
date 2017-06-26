using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.Models.GHTK
{
    public class SuccessOrder
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("order_id")]
        public long OrderId { get; set; }
        [JsonProperty("area")]
        public string Area { get; set; }
        [JsonProperty("fee")]
        public double Fee { get; set; }
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("note")]
        public string Note { get; set; }


    }
}