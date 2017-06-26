using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Theme;

namespace WebApplication.Controllers
{
    public class DisputeResolutionPolicyController : Controller
    {
        //
        // GET: /DisputeResolutionPolicy/

        public ActionResult Index()
        {
            ViewBag.Page = StaticPageBll.GetPage(StaticPageBll.PageTypeName.DisputeResolutionPolicy);
            ViewBag.Url = Url.Action("Index", "DisputeResolutionPolicyController");
            return View(ThemeName.GetView("Article", "Index"));
        }

    }
}
