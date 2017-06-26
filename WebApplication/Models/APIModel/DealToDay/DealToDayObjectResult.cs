using System.Activities.Statements;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebApplication.Models.APIModel.DealToDay
{
    public class DealToDayObjectResult
    {
        [JsonProperty("dealId")]
        public int dealId { get; set; }

        [JsonProperty("dealName")]
        public string dealName { get; set; }

        [JsonProperty("localtionId")]
        public int localtionId { get; set; }

        [JsonProperty("locationName")]
        public string locationName { get; set; }

        [JsonProperty("categoryId")]
        public int categoryId { get; set; }

        [JsonProperty("categoryName")]
        public string categoryName { get; set; }

        [JsonProperty("address")]
        public List<DealToDayAddressResult> address { get; set; }

        [JsonProperty("quantity")]
        public int quantity { get; set; }

        [JsonProperty("originalPrice")]
        public int originalPrice { get; set; }

        [JsonProperty("price")]
        public int price { get; set; }

        [JsonProperty("startDate")]
        public string startDate { get; set; }

        [JsonProperty("expiredDate")]
        public string expiredDate { get; set; }

        [JsonProperty("status")]
        public int status { get; set; }

        [JsonProperty("condition")]
        public string condition { get; set; }

        [JsonProperty("shortDescription")]
        public string shortDescription { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("avatar")]
        public string avatar { get; set; }

        [JsonProperty("lstOtherImage")]
        public List<DealToDayObjectResultImage> lstOtherImage { get; set; }
    }
}