using System.Linq;
using System.Web.Mvc;
using WebApplication.Filters;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.Category;

namespace WebApplication.Controllers
{
    [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
    public class TransactionController : Controller
    {
        //
        // GET: /Transaction/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TransactionHistoryForAdmin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetTransactionHistoryForAdmin(string beginDate, string endDate, int status, string searchType, string searchValue)
        {
            var user = UserBll.GetUser();
            var resultModel = TransactionBll.GetTransHistory(user, beginDate, endDate, status, searchType, searchValue, null);
            ViewBag.TransactionHistory = resultModel;
            return PartialView("tblTransactionHistoryForAdmin");
        }

        [AuthorizeActionFilter]
        [HttpPost]
        public ActionResult GetTransactionHistoryDetailForAdmin(long orderId)
        {
            var user = UserBll.GetUser();
            var resultModel = OrderBll.FindOneOrderDetailsByOrderId(user, orderId);
            ViewBag.TransactionHistoryDetails = resultModel;
            return PartialView("tblTransactionDetail");
        }

        [AuthorizeActionFilter]
        public ActionResult TransactionHistoryForUser()
        {
            var categories = CategoryBll.FindAllCategories();
            ViewBag.Categories = categories.Select(cat => new CategoryManageList(cat)).ToList();

            var user = UserBll.GetUser();
            var resultModel = TransactionBll.GetTransHistoryForUser(user);
            if (user != null)
            {
                ViewBag.User = user;
                ViewBag.TransactionHistory = resultModel;
            }
            else
            {
                ViewBag.User = null;
                ViewBag.TransactionHistory = null;
            }

            return View();
        }
    }
}
