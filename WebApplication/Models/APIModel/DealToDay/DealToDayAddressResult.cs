using System.Activities.Statements;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebApplication.Models.APIModel.DealToDay
{
    public class DealToDayAddressResult
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("address")]
        public string address { get; set; }
    }
}