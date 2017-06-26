using System.Web.Mvc;
using WebApplication.Filters;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.User;

namespace WebApplication.Controllers
{
    [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin, RoleEnum.SupplierManager)]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManageAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoadAccount(string searchKey, string searchValue, int status)
        {
            var user = UserBll.GetUser();

            if (user != null)
            {
                var result = UserBll.ListAllUser(user, searchKey, searchValue, status);

                ViewBag.ResultModel = result;
            }

            return PartialView("tblListAccount");
        }

        [HttpPost]
        public ActionResult UpdateStatus(string email, int status)
        {
            var user = UserBll.GetUser();
            var result = UserBll.UpdateStatusAccount(user, email, status);

            return Json(new { resultCode = result.Code, message = result.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ChangePassword(string email, string passwordNew, string passwordNewConfirm)
        {
            var user = UserBll.GetUser();
            var result = UserBll.ChangePassword(user, email, "", passwordNew, passwordNewConfirm);

            return Json(new { resultCode = result.Code, message = result.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAccount(string email)
        {
            var currentUser = UserBll.GetUser();
            var resultModel = UserBll.GetAccountByEmail(currentUser, email);

            return Json(new { resultCode = resultModel.Code, message = resultModel.Message, data = resultModel.Data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateInfo(UserModel userModel)
        {
            ResultModel result = new ResultModel();
            var user = UserBll.GetUser();

            if (user != null)
            {
                result = UserBll.UpdateInfo(user, userModel);
            }

            return Json(new { resultCode = result.Code, message = result.Message }, JsonRequestBehavior.AllowGet);
        }
    }
}
