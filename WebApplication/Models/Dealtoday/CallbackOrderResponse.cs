using Newtonsoft.Json;
using System;

namespace WebApplication.Models.Dealtoday
{
    [Serializable]
    [JsonObject(Description = "Callback Order Response Json Model")]
    public class CallbackOrderResponse
    {
        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonConstructor]
        public CallbackOrderResponse()
        {

        }
    }
}