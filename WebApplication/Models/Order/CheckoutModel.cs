using System.Collections.Generic;

namespace WebApplication.Models.Order
{
    public class CheckoutModel
    {
        public List<CheckoutItemModel> Items { get; set; }
        public CheckoutDeliveringInfoModel DeliveringInfo { get; set; }
        public int PaymentMethod { get; set; }
    }
}