using Newtonsoft.Json;
using System;

namespace WebApplication.Models.Dealtoday
{
    [Serializable]
    [JsonObject(Description = "Callback Order Request Json Model")]
    public class CallbackOrderRequest
    {
        [JsonProperty(PropertyName = "transaction_id")]
        public long TransactionId { get; set; }

        [JsonProperty("transaction_status")]
        public int TransactionStatus { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonConstructor]
        public CallbackOrderRequest()
        {

        }
    }
}