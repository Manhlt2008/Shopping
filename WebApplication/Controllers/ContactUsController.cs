using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Constant;
using WebApplication.Lib.Util.Theme;
using WebApplication.Models.Category;

namespace WebApplication.Controllers
{
    public class ContactUsController : Controller
    {
        //
        // GET: /ContactUs/

        public ActionResult Index()
        {
            var categories = CategoryBll.FindAllCategories();
            ViewBag.Categories = categories.Select(cat => new CategoryManageList(cat)).ToList();
            ViewBag.Settings =
              SettingsBll.GetSettings(SettingsBll.GetSettingNameGroup(SettingsBll.SettingTypeGroup.ContactUs));
            return View(ThemeName.GetView(ThemeName.ViewName.SettingViewNameEnum.ContactUs));
        }

        [HttpPost]
        public ActionResult SendMailToCompany(string name, string email, string enquiry)
        {
            string contactUsEmail = "";

            var settings = (Dictionary<string, string>)SettingsBll.GetSettings(SettingsBll.GetSettingNameGroup(SettingsBll.SettingTypeGroup.ContactUs));

            settings.TryGetValue(SettingsBll.SettingNames.ContactUsEmail, out contactUsEmail);

            SendMail.SendEmail(Email.CompanyEmail, Email.CompanyPasswordEmail, contactUsEmail, Email.TitleContactUs, enquiry); // Send to company

            SendMail.SendEmail(Email.CompanyEmail, Email.CompanyPasswordEmail, email, Email.TitleContactUs, Email.ContentContactUs); // Send to User

            return RedirectToAction("Index");
        }
    }
}
