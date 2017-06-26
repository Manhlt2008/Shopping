using System.Collections.Generic;

namespace WebApplication.Models.Product
{
    public class ProductThumbnailModelList
    {
        public const string Sale = "Giảm Giá";
        public const string New = "Mới";
        public const string Empty = "";

        public const string Featured = "Nổi Bật";
        public const string Latest = "Mới Nhất";
        public const string Specials = "Đặc Biệt";
        public const string Bestseller = "Mua Nhiều";
        public const string RelatedProducts = "Có Thể Bạn Quan Tâm";

        public List<ProductThumbnailModel> ProductThumbnailModels { get; set; }

        public string Lable { get; set; }

        public string PanelTitle { get; set; }

        public ProductThumbnailModelList()
        {
            ProductThumbnailModels = new List<ProductThumbnailModel>();
            PanelTitle = string.Empty;
            Lable = string.Empty;
        }

        public ProductThumbnailModelList(List<ProductThumbnailModel> productThumbnailModels, string label, string panelTitle)
        {
            ProductThumbnailModels = productThumbnailModels;
            PanelTitle = panelTitle;
            Lable = label;
        }
    }
}