using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Newtonsoft.Json;

namespace WebApplication.Models.APIModel.DealToDay
{
    public class DealToDayRequest
    {
        public const string GetAllEDeal = "getAllEdeal";
        public const string GetEDealInfo = "getEDealinfo";

        [JsonProperty("cmd")]
        public string Cmd { get; set; }

        [JsonProperty("partnerCode")]
        public string PartnerCode { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [DefaultValue(null)]
        [JsonProperty("dealId", NullValueHandling = NullValueHandling.Ignore)]
        public int? DealId { get; set; }

        public NameValueCollection GetNameValueCollection()
        {
            return new NameValueCollection
            {
                {"cmd", Cmd },
                {"partnerCode", PartnerCode},
                {"timestamp", Timestamp},
                {"signature", Signature},
                {"dealId", DealId.ToString()}
            };
        }

        public Dictionary<string, string> GetDictionary()
        {
            return new Dictionary<string, string>
            {
                {"cmd", Cmd },
                {"partnerCode", PartnerCode},
                {"timestamp", Timestamp},
                {"signature", Signature},
                {"dealId", DealId.ToString()}
            };
        }
    }
}