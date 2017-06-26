using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.Models.GHTK
{
    public class ResponseOrdersStatus
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("orders")]
        public List<OrdersTracking> Orders { get; set; }
    }
}