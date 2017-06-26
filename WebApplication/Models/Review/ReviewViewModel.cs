using System;
using WebApplication.Models.Product;
using WebApplication.Models.User;

namespace WebApplication.Models.Review
{
    public class ReviewViewModel
    {
        public long Id { get; set; }

        public string Message { get; set; }

        public long OrderId { get; set; }

        public long OrderDetailId { get; set; }

        public ProductViewModel Product { get; set; }

        public AccountViewModel Account { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Status { get; set; }

        public ReviewViewModel() { }

        public ReviewViewModel(Lib.Dal.DbContext.Review review)
        {
            if (review == null) return;

            Id = review.Id;
            Message = review.ReviewMessage;
            OrderId = review.OrderId;
            OrderDetailId = review.OrderDetailId;
            Product = new ProductViewModel(review.Product);
            Account = new AccountViewModel(review.Account);
            CreatedDate = review.CreatedDate;
            Status = review.Status;
        }
    }
}