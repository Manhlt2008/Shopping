using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.Models.GHTK
{
    public class OrdersTracking
    {
        [JsonProperty("id")]
        public  long SystemId { get; set; }
        [JsonProperty("order_id")]
        public  string DeliveryId { get; set; }
        [JsonProperty("status")]
        public  int Status { get; set; }
        [JsonProperty("reated")]
        public  DateTime Created { get; set; }
        [JsonProperty("updated")]
        public  DateTime updated { get; set; }
    }
}