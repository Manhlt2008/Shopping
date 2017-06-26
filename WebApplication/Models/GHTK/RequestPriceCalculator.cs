using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.Models.GHTK
{
    public class RequestPriceCalculator
    {
        [JsonProperty("pick_province")]
        public string PickProvince { get; set; }
        [JsonProperty("pick_district")]
        public string PickDistrict { get; set; }
        [JsonProperty("pick_ward")]
        public string PickWard { get; set; }
        [JsonProperty("pick_street")]
        public string PickStreet { get; set; }
        [JsonProperty("pick_first_address")]
        public string PickFirstAddress { get; set; }
        [JsonProperty("customer_province")]
        public string CustomerProvince { get; set; }
        [JsonProperty("customer_district")]
        public string CustomerDistrict { get; set; }
        [JsonProperty("customer_ward")]
        public string CustomerWard { get; set; }
        [JsonProperty("customer_street")]
        public string CustomerStreet { get; set; }
        [JsonProperty("customer_first_address")]
        public string CustomerFirstAddress { get; set; }
        [JsonProperty("Weight")]
        public string Weight { get; set; }
    }
}