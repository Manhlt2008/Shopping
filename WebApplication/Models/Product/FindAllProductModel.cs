using System.Collections.Generic;
using WebApplication.Models.Category;
using WebApplication.Models.Stand;

namespace WebApplication.Models.Product
{
    public class FindAllProductModel
    {
        public CategoryManageList Category { get; set; }
        public List<CategoryManageList> Categories { get; set; }
        public List<ProductThumbnailModel> Products { get; set; }
        public string DisplayStyle { get; set; }
        public int Order { get; set; }
        public int TotalProduct { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public string Query { get; set; }
        public string Sort { get; set; }
        public StandModel StandInfor {get;set;}
    }
}