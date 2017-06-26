using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using log4net;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.Category;
using WebApplication.Models.Review;

namespace WebApplication.Models.Product
{
    public class ProductViewModel
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public long Id { get; set; }

        public string Name { get; set; }

        public long CategoryId { get; set; }

        [AllowHtml]
        public string ShortDescription { get; set; }

        [AllowHtml]
        public string Description { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }

        public bool IsFeatured { get; set; }

        public CategoryManageList Category { get; set; }

        public long Cover { get; set; }

        public List<long> Gallery { get; set; }

        public List<ProductReviewModel> Reviews { get; set; }

        public ProductViewModel() { }

        public ProductViewModel(Lib.Dal.DbContext.Product product)
        {
            if (product == null)
            {
                return;
            }

            Id = product.Id;
            Name = product.Name;
            try
            {
                Category = new CategoryManageList(product.Category);
            }
            catch (Exception exception)
            {
                Log.Error("ProductViewModel()", exception);
            }
            CategoryId = product.CategoryId;
            Quantity = product.Quantity;
            ShortDescription = product.ShortDescription;
            Description = product.Description;
            Price = product.Price;
            IsFeatured = product.IsFeatured;

            if (product.Images == null) return;

            var imgCover = product.Images.FirstOrDefault(m => m.Type == ImageTypeEnum.Cover && m.Status == StatusEnum.Active);
            if (imgCover != null)
            {
                Cover = imgCover.Id;
            }

            Gallery = product.Images.Where(m => m.Type == ImageTypeEnum.Gallery && m.Status == StatusEnum.Active).Select(m => m.Id).ToList();

            Reviews = new List<ProductReviewModel>();

            try
            {
                foreach (var review in product.Reviews)
                {
                    Reviews.Add(new ProductReviewModel(review));
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}