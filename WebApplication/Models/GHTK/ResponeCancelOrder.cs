using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.Models.GHTK
{
    public class ResponeCancelOrder
    {
        [JsonProperty("updated_packages")]
        public List<long> UpdatedOrders { get; set; }
        [JsonProperty("invalid_packages")]
        public List<long> InvalidOrders { get; set; }
    }
}