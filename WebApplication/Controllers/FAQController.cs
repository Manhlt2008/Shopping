using System.Web.Mvc;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Theme;

namespace WebApplication.Controllers
{
    public class FAQController : Controller
    {
        //
        // GET: /FAQ/

        public ActionResult Index()
        {
            ViewBag.Page = StaticPageBll.GetPage(StaticPageBll.PageTypeName.Faq);
            ViewBag.Url = Url.Action("Index", "FAQ");
            return View(ThemeName.GetView("Article", "Index"));
        }

    }
}
