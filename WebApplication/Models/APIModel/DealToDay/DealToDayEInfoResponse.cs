using Newtonsoft.Json;

namespace WebApplication.Models.APIModel.DealToDay
{
    public class DealToDayEInfoResponse
    {
        [JsonProperty("result")]
        public DealToDayObjectResult result { get; set; }
    }
}