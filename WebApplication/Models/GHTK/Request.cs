using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.Models.GHTK
{
    public class Request
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("orders")]
        public Orders Orders { get; set; }
     
    }
}