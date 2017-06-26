using System.Web.Mvc;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Theme;

namespace WebApplication.Controllers
{
    public class DeliveryInformationController : Controller
    {
        //
        // GET: /DeliveryInformation/

        public ActionResult Index()
        {
            ViewBag.Page = StaticPageBll.GetPage("Delivery Information");
            ViewBag.Url = Url.Action("Index", "DeliveryInformation");
            return View(ThemeName.GetView("Article", "Index"));
        }

    }
}
