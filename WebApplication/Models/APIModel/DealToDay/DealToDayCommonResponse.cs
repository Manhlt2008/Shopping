using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace WebApplication.Models.APIModel.DealToDay
{
    public class DealToDayCommonResponse
    {
        [DefaultValue("")]
        [JsonProperty("errorCode", NullValueHandling = NullValueHandling.Ignore)]
        public string errorCode { get; set; }

        [DefaultValue("")]
        [JsonProperty("errorMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string errorMessage { get; set; }
        
        [DefaultValue(1)]
        [JsonProperty("totalDeals", DefaultValueHandling = DefaultValueHandling.Populate)]
        public int totalDeals { get; set; }

        [JsonProperty("result")]
        public List<DealToDayObjectResult> result { get; set; }
    }
}