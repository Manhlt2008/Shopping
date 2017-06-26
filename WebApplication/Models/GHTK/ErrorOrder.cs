using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace WebApplication.Models.GHTK
{
    public class ErrorOrder
    {
        [JsonProperty("order")]
        public Order Order { get; set; }
        [JsonProperty("products")]
        public List<Product> Products { get; set; }
        [JsonProperty("messages")]
        public List<string> Messages { get; set; }
        [JsonProperty("exists_orders")]
        public ExistsOrder ExistsOrder { get; set; }

    }


}