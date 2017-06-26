using System;
using Newtonsoft.Json;

namespace WebApplication.Models.Order
{
    public class CheckoutDeliveringInfoModel
    {
        
        public bool IsUsingDefaultAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime Dob { get; set; }
        public string  Gender { get; set; }
        public string  Mail { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
    }
}