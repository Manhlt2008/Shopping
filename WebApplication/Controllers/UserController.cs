using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using WebApplication.Filters;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Constant;
using WebApplication.Lib.Util.Theme;
using WebApplication.Models.Category;
using WebApplication.Models.User;

namespace WebApplication.Controllers
{
    [AuthorizeActionFilter]
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// Info (View / Update)
        /// </summary>
        /// <returns></returns>
       
        public ActionResult Info()
        {
            var categories = CategoryBll.FindAllCategories();
            ViewBag.Categories = categories.Select(cat => new CategoryManageList(cat)).ToList();

            var user = UserBll.GetUser();
            ResultModel resultModel = new ResultModel();
            if (user != null) {
                resultModel = UserBll.GetAccountById(user, user.Id);
                if (resultModel.Code == Result.SUCCESS.Code)
                {
                    ViewBag.User = (UserModel)resultModel.Data;
                }
                else {
                    ViewBag.User = null;
                }
            }
            else
                ViewBag.User = null;

            return View(ThemeName.GetView(ThemeName.ViewName.UserViewNameEnum.Info));
        }

        [HttpPost]
        public ActionResult UpdateInfo(UserModel userModel)
        {
            ResultModel result = new ResultModel();
            var user = UserBll.GetUser();

            if (user != null)
            {
                userModel.Id = user.Id;
                userModel.Email = user.Email;
                result = UserBll.UpdateInfo(user, userModel);
            }

            return Json(new { resultCode = result.Code, message = result.Message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePassword()
        {
            var categories = CategoryBll.FindAllCategories();
            ViewBag.Categories = categories.Select(cat => new CategoryManageList(cat)).ToList();

            var user = UserBll.GetUser();
            ResultModel resultModel = new ResultModel();
            if (user != null)
            {
                resultModel = UserBll.GetAccountById(user, user.Id);
                if (resultModel.Code == Result.SUCCESS.Code)
                {
                    ViewBag.User = (UserModel)resultModel.Data;
                }
                else
                {
                    ViewBag.User = null;
                }
            }
            else
                ViewBag.User = null;

            return View(ThemeName.GetView(ThemeName.ViewName.UserViewNameEnum.ChangePassword));
        }

        [HttpPost]
        public ActionResult ChangePassword(string passwordOld, string passwordNew, string passwordNewConfirm)
        {
            ResultModel result = new ResultModel();
            var user = UserBll.GetUser();

            if (user != null)
            {
                result = UserBll.ChangePassword(user, user.Email, passwordOld, passwordNew, passwordNewConfirm);
            }

            return Json(new { resultCode = result.Code, message = result.Message }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        [HttpPost]
        public JsonResult SearchUser(string query)
        {
            var list = UserBll.Search(query);

            var accountViewModels = list.Select(account => new AccountViewModel(account)).ToList();

            return Json(JsonConvert.SerializeObject(accountViewModels));
        }
    }
}
