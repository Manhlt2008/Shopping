using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Constant;
using WebApplication.Lib.Util.Theme;
using WebApplication.Models.Category;
using WebApplication.Models.User;

namespace WebApplication.Controllers
{
    public class AuthenticationController : Controller
    {
        //
        // GET: /Authentication/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// Login
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            var categories = CategoryBll.FindAllCategories();
            ViewBag.Categories = categories.Select(cat => new CategoryManageList(cat)).ToList();
            if (ControllerContext.HttpContext.Request.UrlReferrer != null && !ControllerContext.HttpContext.Request.UrlReferrer.ToString().Contains("Authentication/Login"))
            {
                Session["PreviousUrl"] = ControllerContext.HttpContext.Request.UrlReferrer.ToString();
            }
            if (UserBll.GetUser() != null)
            {
                var callbackUrl = Session["PreviousUrl"];

                if (callbackUrl != null)
                {
                    return Redirect(callbackUrl.ToString());
                }

                return RedirectToAction("Index", "Home");
            }

            return View(ThemeName.GetView(ThemeName.ViewName.AuthenticationViewNameEnum.Login));
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var result = UserBll.Login(email, password);
            var callbackUrl = Session["PreviousUrl"];
            return Json(new { resultCode = result.Code, message = result.Message, callbackUrl = callbackUrl}, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// Logout
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            HttpContext.Session[MvcApplication.User] = null;
            return RedirectToAction("Login", "Authentication");
        }
        /// <summary>
        /// 
        /// Registration
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            var categories = CategoryBll.FindAllCategories();
            ViewBag.Categories = categories.Select(cat => new CategoryManageList(cat)).ToList();

            return View(ThemeName.GetView(ThemeName.ViewName.AuthenticationViewNameEnum.Register));
        }

        [HttpPost]
        public ActionResult Register(UserModel model)
        {
            ResultModel result = new ResultModel();

            result = UserBll.CreateUser(model);
            if (result.Code.Equals(Result.SUCCESS.Code))
            {
                UserBll.Login(model.Email, model.Password);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Register", "Authentication");
        }

        [HttpPost]
        public ActionResult IsValidEmail(string email)
        {
            ResultModel result = new ResultModel();

            if (!UserBll.isExistedEmail(email))
                result.setCode(Result.SUCCESS);
            else
                result.setCode(Result.FAILED);

            return Json(new { resultCode = result.Code, message = result.Message }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// Forgot password
        /// </summary>
        /// <returns></returns>
        public ActionResult ForgotPassword()
        {
            var categories = CategoryBll.FindAllCategories();
            ViewBag.Categories = categories.Select(cat => new CategoryManageList(cat)).ToList();

            return View(ThemeName.GetView(ThemeName.ViewName.AuthenticationViewNameEnum.ForgotPassword));
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            var reader=new StreamReader(Server.MapPath("~/Common/EmailForgetPassword.html"));
            var result = UserBll.ForgotPassword(email, reader);

            return Json(new { resultCode = result.Code, message = result.Message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// Forgot password
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetPassword(string email, string token)
        {
            var categories = CategoryBll.FindAllCategories();
            ViewBag.Categories = categories.Select(cat => new CategoryManageList(cat)).ToList();

            ResultModel result = new ResultModel();

            result = UserBll.isValidTokenForgotPassword(email, token);

            ViewBag.ResetPassword = result;
            ViewBag.Email = email;
            ViewBag.Token = token;

            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(string email, string token, string passwordNew, string passwordNewConfirm)
        {

            ResultModel result = new ResultModel();

            result = UserBll.ResetPassword(email, token, passwordNew, passwordNewConfirm);

            return Json(new { resultCode = result.Code, message = result.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string Ack()
        {
            return UserBll.GetUser() == null ? "0" : "1";
        }
    }
}
