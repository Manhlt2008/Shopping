using System.Web.Mvc;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Theme;

namespace WebApplication.Controllers
{
    public class ReturnController : Controller
    {
        //
        // GET: /Return/

        public ActionResult Index()
        {
            ViewBag.Page = StaticPageBll.GetPage(StaticPageBll.PageTypeName.Returns);
            ViewBag.Url = Url.Action("Index", "Return");
            return View(ThemeName.GetView("Article", "Index"));
        }

    }
}
