using System.Web.Mvc;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Theme;

namespace WebApplication.Controllers
{
    public class TermAndConditionsController : Controller
    {
        //
        // GET: /TermAndConditions/

        public ActionResult Index()
        {
            ViewBag.Page = StaticPageBll.GetPage("Term and Conditions");
            ViewBag.Url = Url.Action("Index", "PrivacyPolicy");
            return View(ThemeName.GetView("Article", "Index"));
        }

    }
}
