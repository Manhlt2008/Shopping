using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.Models.GHTK
{
    public class Reponse
    {
        [JsonProperty("success")]
        public bool IsSucess { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("success_orders")]
        public List<SuccessOrder> SuccessOrders { get; set; }
        [JsonProperty("error_orders")]
        public List<ErrorOrder> ErrorOrders { get; set; }


    }
}