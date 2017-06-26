using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.Models.APIModel.DealToDay
{
    public class DealToDayCreateOrderResponse
    {
        [DefaultValue("")]
        [JsonProperty("errorCode", NullValueHandling = NullValueHandling.Ignore)]
        public string errorCode { get; set; }

        [DefaultValue("")]
        [JsonProperty("errorMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string errorMessage { get; set; }

        [JsonProperty("result")]
        public DealToDayCreateObjectResult result { get; set; }
    }
}