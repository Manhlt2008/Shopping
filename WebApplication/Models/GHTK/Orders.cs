using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.Models.GHTK
{
    public class Orders
    {
        [JsonProperty("products")]
        public List<Product> Products { get; set; }
        [JsonProperty("order")]
        public Order Order { get; set; }
    }
}