using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.Models.APIModel.DealToDay
{
    public class DealToDayCreateOrder
    {
        public const string cmd = "createOrder";
        public const string partnerCode = "PHUNUMART";

        [JsonProperty("cmd")]
        public string Cmd { get; set; }

        [JsonProperty("partnerCode")]
        public string PartnerCode { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("orderCode")]
        public string OrderCode { get; set; }

        [JsonProperty("createdDate")]
        public string CreatedDate { get; set; }

        [JsonProperty("paymentStatus")]
        public int PaymentStatus { get; set; }

        [JsonProperty("paymentName")]
        public string PaymentName { get; set; }

        [JsonProperty("totalAmount")]
        public int TotalAmount { get; set; }

        [JsonProperty("products")]
        public List<DealToDayProduct> Products { get; set; }

        [JsonProperty("customerFullName")]
        public string CustomerFullName { get; set; }

        [JsonProperty("customerMobile")]
        public string CustomerMobile { get; set; }

        [JsonProperty("customerAddress")]
        public string CustomerAddress { get; set; }

        [JsonProperty("customerEmail")]
        public string CustomerEmail { get; set; }

        [JsonProperty("customerGender")]
        public int CustomerGender { get; set; }

        public NameValueCollection GetNameValueCollection()
        {
            return new NameValueCollection
            {
                {"cmd", Cmd },
                {"partnerCode", PartnerCode},
                {"timestamp", Timestamp},
                {"orderCode", OrderCode},
                {"createdDate", CreatedDate },
                {"paymentStatus", PaymentStatus.ToString()},
                {"paymentName",PaymentName },
                {"totalAmount",TotalAmount.ToString() },
                {"products" ,Products.ToString()},
                {"customerFullName", CustomerFullName },
                {"customerMobile", CustomerMobile },
                {"customerAddress", CustomerAddress },
                {"customerEmail",CustomerEmail },
                {"customerGender",CustomerGender.ToString() }
            };
        }
        public Dictionary<string, string> getDictionary()
        {
            return new Dictionary<string, string>
            {
                {"cmd", Cmd },
                {"partnerCode", PartnerCode},
                {"timestamp", Timestamp},
                {"orderCode", OrderCode},
                {"createdDate", CreatedDate },
                {"paymentStatus", PaymentStatus.ToString()},
                {"paymentName",PaymentName },
                {"totalAmount",TotalAmount.ToString() },
                {"products" ,Products.ToString()},
                {"customerFullName", CustomerFullName },
                {"customerMobile", CustomerMobile },
                {"customerAddress", CustomerAddress },
                {"customerEmail",CustomerEmail },
                {"customerGender",CustomerGender.ToString() }
            };
        }
    }
}