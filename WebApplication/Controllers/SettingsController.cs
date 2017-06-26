using System.Web;
using System.Web.Mvc;
using WebApplication.Filters;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Theme;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.Setiings;

namespace WebApplication.Controllers
{
    [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
    public class SettingsController : BaseController
    {
        //
        // GET: /Settings/

        public ActionResult Index()
        {
            return View();
        }

        #region [Footer]
        public ActionResult Footer()
        {
            ViewBag.IsSaveSuccess = TempData["IsSaveSuccess"];
            ViewBag.Settings =
                SettingsBll.GetSettings(SettingsBll.GetSettingNameGroup(SettingsBll.SettingTypeGroup.Footer));
            return View();
        }

        [HttpPost]
        public ActionResult Footer(FooterModel model)
        {
            TempData["IsSaveSuccess"] = SettingsBll.SaveFooter(model);

            return RedirectToAction("Footer");
        }

        #endregion

        public ActionResult Article(string type)
        {
            var page = StaticPageBll.GetPage(type);

            if (page == null)
            {
                throw new HttpException(404, "Article Not Found");
            }
            ViewBag.Page = page;
            ViewBag.Type = type;
            ViewBag.IsSuccess = TempData["IsSuccess"];
            return View();
        }

        [HttpPost]
        public ActionResult Article(ArticleModel articleModel)
        {
            var isSuccess = StaticPageBll.Update(articleModel);
            TempData["IsSuccess"] = isSuccess;
            return RedirectToAction("Article", "Settings", new { type = articleModel.TypeName });
        }


        #region [Contact US]
        public ActionResult ContactUs()
        {
            ViewBag.IsSaveSuccess = TempData["IsSaveSuccess"];
            ViewBag.Settings =
                SettingsBll.GetSettings(SettingsBll.GetSettingNameGroup(SettingsBll.SettingTypeGroup.ContactUs));
            return View(ThemeName.GetView(ThemeName.ViewName.SettingViewNameEnum.ContactUs));
        }

        [HttpPost]
        public ActionResult ContactUs(ContactUsModel model)
        {
            TempData["IsSaveSuccess"] = SettingsBll.SaveContactUs(model);

            return RedirectToAction("ContactUs");
        }

        #endregion

        #region [Order]

        public ActionResult Order()
        {
            ViewBag.IsSaveSuccess = TempData["IsSaveSuccess"];
            ViewBag.Settings =
                SettingsBll.GetSettings(SettingsBll.GetSettingNameGroup(SettingsBll.SettingTypeGroup.Order));
            return View();
        }

        [HttpPost]
        public ActionResult Order(OrderModel model)
        {
            TempData["IsSaveSuccess"] = SettingsBll.SaveOrder(model);

            return RedirectToAction("Order");
        }
        #endregion
    }
}
