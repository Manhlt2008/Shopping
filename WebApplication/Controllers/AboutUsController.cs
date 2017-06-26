using System.Web.Mvc;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Theme;

namespace WebApplication.Controllers
{
    public class AboutUsController : Controller
    {
        //
        // GET: /AboutUs/

        public ActionResult Index()
        {
            ViewBag.Page = StaticPageBll.GetPage(StaticPageBll.PageTypeName.AboutUs);
            ViewBag.Url = Url.Action("Index", "AboutUs");
            return View(ThemeName.GetView("Article", "Index"));
        }

    }
}
