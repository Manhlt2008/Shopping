using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Lib.Util.Constant;

namespace WebApplication.Models.Product
{
    public class ProductModel
    {
        public long Id { get; set; }

        [Display(Name = "Tên Sản Phẩm")]
        public string Name { get; set; }

        [Display(Name = "Danh Mục")]
        public long CategoryId { get; set; }

        [Display(Name = "Mô Tả Ngắn")]
        [AllowHtml]
        public string ShortDescription { get; set; }

        [Display(Name = "Mô Tả")]
        [AllowHtml]
        public string Description { get; set; }

        [Display(Name = "Giá")]
        public double Price { get; set; }

        [Display(Name = "Số Lượng")]
        public int Quantity { get; set; }

        [Display(Name = "Is Featured")]
        public bool IsFeatured { get; set; }

        [Display(Name = "Hình")]
        public HttpPostedFileBase Cover { get; set; }

        [Display(Name = "Thư Viện")]
        public List<HttpPostedFileBase> Gallery { get; set; }

        public long CoverId { get; set; }

        public string CoverImage { get; set; }

        public byte Status { get; set; }

        public List<long> GalleryIds { get; set; }

        public List<string> GalleryImages { get; set; }

        public long SupplierId { get; set; }
        public ProductModel()
        {
            Id = -1;
            Name = string.Empty;
            CategoryId = 0;
            ShortDescription = string.Empty;
            Description = string.Empty;
            Price = 0;
            Quantity = 0;
            IsFeatured = true;
            Cover = null;
            Gallery = new List<HttpPostedFileBase>();
            CoverId = -1;
            CoverImage = string.Empty;
            GalleryIds = Enumerable.Empty<long>().ToList();
            GalleryImages = new List<string>();
        }

        public ProductModel(Lib.Dal.DbContext.Product product)
        {
            if (product != null)
            {
                Id = product.Id;
                Name = product.Name;
                CategoryId = product.CategoryId;
                ShortDescription = product.ShortDescription;
                Description = product.Description;
                Price = product.Price;
                Quantity = product.Quantity;
                IsFeatured = true;
                Cover = null;
                Gallery = new List<HttpPostedFileBase>();

                var cover = product.Images.FirstOrDefault(m => m.Type == ImageTypeEnum.Cover && m.Status == StatusEnum.Active);
                var gallary = product.Images.Where(m => m.Type == ImageTypeEnum.Gallery && m.Status == StatusEnum.Active).ToList();

                CoverId = cover != null ? cover.Id : -1;
                GalleryIds = gallary.Select(m => m.Id).ToList();
            }
        }
    }
}