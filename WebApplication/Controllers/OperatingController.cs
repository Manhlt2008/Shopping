using System.Web.Mvc;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Theme;

namespace WebApplication.Controllers
{
    public class OperatingController : Controller
    {
        //
        // GET: /Operating/

        public ActionResult Index()
        {
            ViewBag.Page = StaticPageBll.GetPage(StaticPageBll.PageTypeName.Operating);
            ViewBag.Url = Url.Action("Index", "Operating");
            return View(ThemeName.GetView("Article", "Index"));
        }

    }
}
