using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using WebApplication.Models.OrderDetail;
using WebApplication.Models.Product;
using WebApplication.Models.User;

namespace WebApplication.Models.Order
{
    public class OrderOverviewInfoResponseModel : ISerializable
    {
        public long OrderId { get; set; }

        public DateTime CreatedDate { get; set; }

        public double TotalPrice { get; set; }

        public int Status { get; set; }

        public string Code { get; set; }
        
        public AccountViewModel Account { get; set; }

        public List<OrderDetailModel> OrderDetail { get; set; }

        public OrderOverviewInfoResponseModel() { }
        public OrderOverviewInfoResponseModel(Lib.Dal.DbContext.Order order, bool isLoadOrderDetail = false)
        {
            if (order != null)
            {
                OrderId = order.Id;
                CreatedDate = order.CreatedDate;
                TotalPrice = order.TotalPrice;
                Status = order.Status;
                Code = order.Code;
                Account = new AccountViewModel(order.Account);

                if (isLoadOrderDetail)
                {
                    OrderDetail = new List<OrderDetailModel>();
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        OrderDetail.Add(new OrderDetailModel(orderDetail));
                    }
                }
                else
                {
                    OrderDetail = Enumerable.Empty<OrderDetailModel>().ToList();
                }
            }
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("OrderId", OrderId);
            info.AddValue("CreatedDate", CreatedDate);
            info.AddValue("TotalPrice", TotalPrice);
            info.AddValue("Status", Status);
            info.AddValue("Code", Code);
            info.AddValue("Account", Account);
            info.AddValue("OrderDetail", OrderDetail);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            GetObjectData(info, context);
        }

    }
}