using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Theme;
using WebApplication.Models.Category;

namespace WebApplication.Controllers
{
    public class ErrorController : Controller
    {
        private void Init()
        {
            var categories = CategoryBll.FindAllCategories();
            ViewBag.Categories = categories.Select(cat => new CategoryManageList(cat)).ToList();
        }

        public ActionResult Index()
        {
            throw new HttpException(404, "Page not found");
        }

        public ActionResult PageNotFound()
        {
            Init();
            ViewBag.Title = "404 - Trang Không Tồn Tại";
            ViewBag.Description = "Thật xin lỗi trang bạn đang tìm không tồn tại trên hệ thống.";
            ViewBag.ErrorCode = 404;
            return View(ThemeName.GetView(ThemeName.ViewName.ErrorViewNameEnum.PageNotFound));
        }

        public ActionResult AccessDenied()
        {
            Init();
            ViewBag.Title = "403 - Từ Chối Truy Cập";
            ViewBag.Description = "Bạn không có quyền truy xuất vào trang này của hệ thống. Vui lòng liên hệ webmaster để biết thêm tin chi tiết.";
            Response.Headers.Add("HTTPCode", "403");
            ViewBag.ErrorCode = 403;
            return View(ThemeName.GetView(ThemeName.ViewName.ErrorViewNameEnum.PageNotFound));
        }

        public ActionResult ServiceUnavailable()
        {
            Init();
            ViewBag.Title = "503 - Service Unavailable";
            ViewBag.Description = "Không thể kết nối tới server. Vui lòng thử lại sau.";
            ViewBag.ErrorCode = 503;
            return View(ThemeName.GetView(ThemeName.ViewName.ErrorViewNameEnum.PageNotFound));
        }
    }
}
