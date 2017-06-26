using System.Web.Mvc;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Theme;

namespace WebApplication.Controllers
{
    public class PrivacyPolicyController : Controller
    {
        //
        // GET: /PrivacyPolicy/

        public ActionResult Index()
        {
            ViewBag.Page = StaticPageBll.GetPage("Privacy Policy");
            ViewBag.Url = Url.Action("Index", "PrivacyPolicy");
            return View(ThemeName.GetView("Article", "Index"));
        }

    }
}
