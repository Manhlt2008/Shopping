using System.Collections.Generic;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace WebApplication.Models.APIModel._123pay
{
    public class Create123PayRequestModel
    {
        [JsonProperty("mTransactionID")]
        public string MTransactionId { get; set; }

        [JsonProperty("merchantCode")]
        public string MerchantCode { get; set; }

        [JsonProperty("bankCode")]
        public string BankCode { get; set; }

        [JsonProperty("totalAmount")]
        public double TotalAmount { get; set; }

        [JsonProperty("clientIP")]
        public string ClientIp { get; set; }

        [JsonProperty("custName")]
        public string CustomerName { get; set; }

        [JsonProperty("custAddress")]
        public string CustomerAddress { get; set; }

        [JsonProperty("custGender")]
        public string CustomerGender { get; set; }

        [JsonProperty("custDOB")]
        public string CustomerDob { get; set; }

        [JsonProperty("custPhone")]
        public string CustomerPhone { get; set; }

        [JsonProperty("custMail")]
        public string CustomerMail { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("cancelURL")]
        public string CancelUrl { get; set; }

        [JsonProperty("redirectURL")]
        public string RedirectUrl { get; set; }

        [JsonProperty("errorURL")]
        public string ErrorUrl { get; set; }

        [JsonProperty("passcode")]
        public string Passcode { get; set; }

        [JsonProperty("checksum")]
        public string Checksum { get; set; }

        [JsonProperty("addInfo")]
        public string AddInfo { get; set; }

        [JsonProperty("ReturnCode")]
        public string ReturnCode { get; set; }

        [JsonProperty("Ts")]
        public string Ts { get; set; }

        [JsonProperty("ReturnDescription")]
        public string ReturnDescription { get; set; }
        
        [JsonProperty("OpAmount")]
        public string OpAmount { get; set; }
        public NameValueCollection GetNameValueCollection()
        {
            return new NameValueCollection
            {
                {"mTransactionID", MTransactionId},
                {"merchantCode", MerchantCode},
                {"bankCode", BankCode},
                {"totalAmount", TotalAmount.ToString("####")},
                {"clientIP", ClientIp},
                {"custName", CustomerName},
                {"custAddress", CustomerAddress},
                {"custGender", CustomerGender},
                {"custDOB", CustomerDob},
                {"custPhone", CustomerPhone},
                {"custMail", CustomerMail},
                {"description", Description},
                {"cancelURL", CancelUrl},
                {"redirectURL", RedirectUrl},
                {"errorURL", ErrorUrl},
                {"checksum", Checksum},
                {"addInfo", AddInfo},
                {"passcode", Passcode }
            };
        }

        public Dictionary<string, string> GetDictionary()
        {
            return new Dictionary<string, string>
            {
                {"mTransactionID", MTransactionId},
                {"merchantCode", MerchantCode},
                {"bankCode", BankCode},
                {"totalAmount", TotalAmount.ToString("####")},
                {"clientIP", ClientIp},
                {"custName", CustomerName},
                {"custAddress", CustomerAddress},
                {"custGender", CustomerGender},
                {"custDOB", CustomerDob},
                {"custPhone", CustomerPhone},
                {"custMail", CustomerMail},
                {"description", Description},
                {"cancelURL", CancelUrl},
                {"redirectURL", RedirectUrl},
                {"errorURL", ErrorUrl},
                {"checksum", Checksum},
                {"addInfo", AddInfo},
                {"passcode", Passcode }
            };
        }
    }
}