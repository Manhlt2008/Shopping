using Newtonsoft.Json;

namespace WebApplication.Models.Order
{
    public class CheckoutItemModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}