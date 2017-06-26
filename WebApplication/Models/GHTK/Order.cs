using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication.Models.GHTK
{
    public class Order
    { 
        [JsonProperty("id")]
        public long SystemOrderId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("province")]
        public string Province { get; set; }
        [JsonProperty("district")]
        public string District { get; set; }
        [JsonProperty("ward")]
        public string Ward { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("tel")]
        public string Tel { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("pick_money")]
        public double PickMoney { get; set; }
        [JsonProperty("pick_name")]
        public string PickName { get; set; }
        [JsonProperty("pick_address_id")]
        public long PickAddressId { get; set; }
        [JsonProperty("pick_address")]
        public string PickAddress { get; set; }
        [JsonProperty("pick_province")]
        public string PickProvince { get; set; }
        [JsonProperty("pick_district")]
        public string PickDistrict { get; set; }
        [JsonProperty("pick_ward")]
        public string PickWard { get; set; }
        [JsonProperty("pick_street")]
        public string PickStreet { get; set; }
        [JsonProperty("pick_tel")]
        public string PickTel { get; set; }
        [JsonProperty("pick_email")]
        public string PickEmail { get; set; }
        [JsonProperty("note")]
        public string Note { get; set; }
        [JsonProperty("is_freeship")]
        public int IsFreeship { get; set; }
        [JsonProperty("deliver_work_shift")]
        public int DeliverWorkShift { get; set; }
    }
}
  