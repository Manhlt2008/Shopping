using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.Models.GHTK
{
    public class CheckOrdersStatus
    {
        [JsonProperty("token")]
        public  string Token { get; set; }
        [JsonProperty("ids")]
        public  List<long> Ids { get; set; }
        [JsonProperty("type")]
        public  int Type { get; set; }
    }
}