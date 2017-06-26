using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace WebApplication.Models.Order
{
    public class OrderOverviewInfoResponseModelList : ISerializable
    {
        public List<OrderOverviewInfoResponseModel> Orders { get; set; }

        public OrderOverviewInfoResponseModelList(List<Lib.Dal.DbContext.Order> orderList)
        {
            if (orderList != null)
            {
                Orders = new List<OrderOverviewInfoResponseModel>();

                foreach (var order in orderList)
                {
                    Orders.Add(new OrderOverviewInfoResponseModel(order));
                }
            }
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Orders", Orders);
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