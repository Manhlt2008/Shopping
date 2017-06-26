using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.Models.APIModel.DealToDay
{
    public class DealToDayTrackingOrder
    {
        public const string cmd = "createOrder";
        public const string partnerCode = "PHUNUMART";

        [JsonProperty("cmd")]
        public string Cmd { get; set; }

        [JsonProperty("partnerCode")]
        public string PartnerCode { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
    }
}