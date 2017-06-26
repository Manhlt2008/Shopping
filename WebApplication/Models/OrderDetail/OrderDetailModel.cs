using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Security.Permissions;
using WebApplication.Models.User;

namespace WebApplication.Models.OrderDetail
{
    public class OrderDetailModel : ResultModel, ISerializable
    {
        [Display(Name = "Id")]
        public long Id { get; set; }
        [Display(Name = "ProductId")]
        public long ProductId { get; set; }
        [Display(Name = "OrderId")]
        public long OrderId { get; set; }
        [Display(Name = "OriginUnitPrice")]
        public double OriginUnitPrice { get; set; }
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        [Display(Name = "Discount")]
        public double Discount { get; set; }
        [Display(Name = "ProductName")]
        public string ProductName { get; set; }
        [Display(Name = "CategoryName")]
        public string CategoryName { get; set; }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]

        public OrderDetailModel() { }
        
        public OrderDetailModel(Lib.Dal.DbContext.OrderDetail orderDetail)
        {
            if (orderDetail != null)
            {
                Id = orderDetail.Id;
                ProductId = orderDetail.ProductId;
                OrderId = orderDetail.OrderId;
                OriginUnitPrice = orderDetail.OriginUnitPrice;
                Quantity = orderDetail.Quantity;
                Discount = orderDetail.Discount;
                ProductName = orderDetail.Product.Name;
                CategoryName = orderDetail.Product.Category.Name;
            }
        }

        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("ProductId", ProductId);
            info.AddValue("OrderId", OrderId);
            info.AddValue("OriginUnitPrice", OriginUnitPrice);
            info.AddValue("Quantity", Quantity);
            info.AddValue("Discount", Discount);
            info.AddValue("ProductName", ProductName);
            info.AddValue("CategoryName", CategoryName);
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