using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.Models.APIModel.DealToDay
{
    public class DealToDayCreateObjectResult
    {
        [JsonProperty("transactionId")]
        public int TransactionId { get; set; }
        [JsonProperty("totalAmount")]
        public int TotalAmount { get; set; }
    }
}