using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Web.Mvc;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.Category;
using SupplierProduct = WebApplication.Lib.Dal.DbContext.SupplierProduct;

namespace WebApplication.Models.Product
{
    public class ProductThumbnailModel : ISerializable
    {
        public long Id { get; set; }

        public long? SupplierId { get; set; }

        public string Name { get; set; }

        public CategoryModel Category { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }

        public bool IsFeatured { get; set; }

        [AllowHtml]
        public string Description { get; set; }

        [AllowHtml]
        public string ShortDescription { get; set; }

        public long Cover { get; set; }

        public int Status { get; set; }

        public List<long> Gallery { get; set; }

        public List<SupplierProduct> SupplierProducts { get; set; }

        public ProductModel UpdatedProduct { get; set; }

        public ProductThumbnailModel(Lib.Dal.DbContext.Product product)
        {
            if (product == null)
            {
                return;
            }

            var imgCover = product.Images.FirstOrDefault(m => m.Type == ImageTypeEnum.Cover && m.Status == StatusEnum.Active);

            Id = product.Id;
            Name = product.Name;
            Category = new CategoryModel(product.Category);
            Price = product.Price;
            Quantity = product.Quantity;
            IsFeatured = product.IsFeatured;
            Description = product.Description;
            ShortDescription = product.ShortDescription;

            if (imgCover != null)
            {
                Cover = imgCover.Id;
            }
            Gallery =
                product.Images.Where(m => m.Type == ImageTypeEnum.Gallery && m.Status == StatusEnum.Active)
                    .Select(m => m.Id)
                    .ToList();
            Status = product.Status;

            try
            {
                var supplierProduct = product.SupplierProducts.FirstOrDefault();

                if (supplierProduct != null)
                {
                    SupplierId = supplierProduct.SupplierId;

                    if (!string.IsNullOrEmpty(supplierProduct.ExtraInfo))
                    {
                        UpdatedProduct = JsonConvert.DeserializeObject<ProductModel>(supplierProduct.ExtraInfo);
                    }
                }
            }
            catch
            {
                // Do nothing
            }
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("Name", Name);
            info.AddValue("Category", Category);
            info.AddValue("Price", Price);
            info.AddValue("Quantity", Quantity);
            info.AddValue("IsFeatured", IsFeatured);
            info.AddValue("Description", Description);
            info.AddValue("ShortDescription", ShortDescription);
            info.AddValue("Cover", Cover);

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