
using Newtonsoft.Json;

namespace WebApplication.Models.APIModel._123pay
{
    public class Create123PayResponseModel
    {
        [JsonProperty("returnCode")]
        public string ReturnCode { get; set; }

        [JsonProperty("123PayTransactionId")]
        public string TransactionId { get; set; }

        [JsonProperty("redirectURL")]
        public string RedirectUrl { get; set; }

        [JsonProperty("checksum")]
        public string Checksum { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}