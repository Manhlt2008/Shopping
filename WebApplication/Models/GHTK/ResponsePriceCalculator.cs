using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.Models.GHTK
{
    public class ResponsePriceCalculator
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("fee")]
        public long Fee { get; set; }
        [JsonProperty("delivery_type")]
        public string DeliveryType { get; set; }
        [JsonProperty("msg")]
        public string Message { get; set; }
    }
}