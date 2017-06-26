namespace WebApplication.Models.Review
{
    public class UpdateAndApproveSaveReviewModel
    {
        public long ReviewId { get; set; }

        public string Message { get; set; }

        public int Status { get; set; }
    }
}