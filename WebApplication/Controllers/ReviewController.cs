using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Filters;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.Product;
using WebApplication.Models.Review;
using WebApplication.Models.User;

namespace WebApplication.Controllers
{
    public class ReviewController : Controller
    {
        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        private void FindAllReviews(long productId, string sort = "default", int order = 1, int limit = 6, int page = 1)
        {
            var products = ProductBll.FindAllProductHavingReview();
            var product = ProductBll.FindById(productId);
            int totalReview;
            var reviews = ReviewBll.FindByProductId(productId, sort, order, limit, page, out totalReview);

            if (productId != 0 && product == null)
            {
                throw new HttpException(500, "Product Not Found.");
            }

            if (productId == 0)
            {
                product = new Product
                {
                    Id = 0,
                    Name = "All Products"
                };
            }

            ViewBag.Product = new ProductViewModel(product);
            ViewBag.Products = products.Select(p => new ProductViewModel(p)).ToList();
            ViewBag.Reviews = reviews.Select(r => new ReviewViewModel(r)).ToList();
            ViewBag.DisplayStyle = "grid";
            ViewBag.Sort = sort;
            ViewBag.Order = order;
            ViewBag.TotalReview = totalReview;
            ViewBag.Page = page;
            ViewBag.Limit = limit;
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Admin");
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        public ActionResult Manage(long productId = 0, string sort = "All", int order = 1, int limit = 5, int page = 1)
        {
            FindAllReviews(productId, sort, order, limit, page);
            ViewBag.ProductId = productId;

            return View();
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        [HttpPost]
        public JsonResult Approve(long reviewId)
        {
            var user = UserBll.GetUser();

            var isSuccess = ReviewBll.UpdateStatus(reviewId, StatusEnum.ReviewEnum.Status.Approve, user);

            return Json(new { IsSuccess = isSuccess });
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        [HttpPost]
        public JsonResult ApproveAndUpdate(UpdateAndApproveSaveReviewModel reviewModel)
        {
            var status = ReviewBll.UpdateAndApprove(reviewModel, UserBll.GetUser());
            return Json(new { IsSuccess = status });
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        [HttpPost]
        public JsonResult DeleteReview(long reviewId)
        {
            var user = UserBll.GetUser();
            if (user == null || (user.RoleId != RoleEnum.Admin) && (user.RoleId != RoleEnum.Manager))
            {
                return Json(new {IsSuccess = false, Message = "Access Denied", Code = 0});
            }

            var status = ReviewBll.Delete(reviewId, user);
            return Json(new { IsSuccess = status });
        }
    }
}
