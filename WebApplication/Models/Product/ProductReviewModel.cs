using System.Web.Mvc;
using WebApplication.Models.User;

namespace WebApplication.Models.Product
{
    public class ProductReviewModel
    {
        public long ReviewId { get; set; }
        [AllowHtml]
        public string ReviewMessage { get; set; }

        public string Code { get; set; }

        public long ProductId { get; set; }

        public int Status { get; set; }

        public UserModel User { get; set; }

        public ProductReviewModel() { }

        public ProductReviewModel(Lib.Dal.DbContext.Review review)
        {
            if (review != null)
            {
                ReviewId = review.Id;
                ReviewMessage = review.ReviewMessage;
                ProductId = review.ProductId;
                Status = review.Status;
                User = new UserModel(review.Account);
            }
        }
    }
}