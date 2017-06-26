using Newtonsoft.Json;
using System;

namespace WebApplication.Models.Dealtoday
{
    [Serializable]
    [JsonObject(Description = "Publish Order Request Json Model")]
    public class PublishOrderRequest
    {
        [JsonProperty(PropertyName = "order_code", Required = Required.DisallowNull)]
        public string OrderCode { get; set; }

        [JsonProperty("created_date")]
        public DateTime CreateddDate { get; set; }

        [JsonProperty("payment_status")]
        public byte PaymentStatus { get; set; }

        [JsonProperty("products")]
        public string Products { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("unit_price")]
        public int UnitPrice { get; set; }

        [JsonProperty("customer_fullname")]
        public string CustomerFullname { get; set; }

        [JsonProperty("customer_phone")]
        public string CustomerPhone { get; set; }

        [JsonProperty("customer_address")]
        public string CustomerAddress { get; set; }

        [JsonProperty("customer_mail")]
        public string CustomerMail { get; set; }

        [JsonProperty("customer_gender")]
        public byte CustomerGender { get; set; }

        [JsonConstructor]
        public PublishOrderRequest()
        {

        }
    }
}