using System.Collections.Generic;
using WebApplication.Lib.Util.Constant;

namespace WebApplication.Models.Order
{
    public class OrderStatusModel
    {
        public int Value { get; set; }
        public string Text { get; set; }

        public OrderStatusModel() { }

        public static List<OrderStatusModel> GetList()
        {
            return new List<OrderStatusModel>
            {
                new OrderStatusModel{Value = OrderStatusEnum.New, Text = "New"},
                new OrderStatusModel{Value = OrderStatusEnum.Delevering, Text = "Delevering"},
                new OrderStatusModel{Value = OrderStatusEnum.Purchased, Text = "Purchased"},
                new OrderStatusModel{Value = OrderStatusEnum.Completed, Text = "Completed"},
                new OrderStatusModel{Value = OrderStatusEnum.Reject, Text = "Reject"},
            };
        }
    }
}