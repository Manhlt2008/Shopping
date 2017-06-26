using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.Models.GHTK
{
    public class CancelOrders
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("orders")]
        public List<long> SystemIds { get; set; }
    }
}