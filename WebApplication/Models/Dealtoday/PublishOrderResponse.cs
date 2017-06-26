using Newtonsoft.Json;
using System;

namespace WebApplication.Models.Dealtoday
{
    [Serializable]
    [JsonObject(Description = "Publish Order Response Json Model")]
    public class PublishOrderResponse
    {
        [JsonProperty(PropertyName ="transaction_id")]
        public long TransactionId { get; set; }

        [JsonProperty("extra_info")]
        public string ExtraInfo { get; set; }

        [JsonConstructor]
        public PublishOrderResponse()
        {

        }
    }
}