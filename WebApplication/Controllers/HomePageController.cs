using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApplication.Filters;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.HomPage;

namespace WebApplication.Controllers
{
    [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
    public class HomePageController : Controller
    {
        //
        // GET: /HomePage/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetHomePageProduct(long productId)
        {
            var user = UserBll.GetUser();
            var resultModel = HomePageBll.getHomePageByProductId(user, productId);
            ViewBag.ResultModel = resultModel;

            return PartialView("ModalPartial/ModalSetHomePage", resultModel);

        }

        [HttpPost]
        public ActionResult SaveListHomePages(string homepages)
        {
            var user = UserBll.GetUser();
            var listHomePage = new List<HomePageModel>();
            var jsSerializer = new JavaScriptSerializer();
            listHomePage = jsSerializer.Deserialize<List<HomePageModel>>(homepages);
            var resultModel = HomePageBll.SaveListHomePages(listHomePage);

            return Json(new { resultCode = resultModel.Code, message = resultModel.Message }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult SaveHomePage(string homepage)
        {
            var user = UserBll.GetUser();
            var home = new HomePageModel();
            var jsSerializer = new JavaScriptSerializer();
            home = jsSerializer.Deserialize<HomePageModel>(homepage);
            home.StartDate = DateTime.Now;
            home.EndDate = DateTime.Now;
            var resultModel = HomePageBll.SaveHomePage(home);

            return Json(new { resultCode = resultModel.Code, message = resultModel.Message }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Manage()
        {
            var user = UserBll.GetUser();
            var resultModel = HomePageBll.getHomePageByStatus((int)StatusEnum.Active, TypeHomePageEnum.AllTypeHomePage);
            ViewBag.ResultModel = resultModel;

            return View();
        }

        [HttpPost]
        public ActionResult GetHomePageByTypeAndProductId(string homepage)
        {
            var user = UserBll.GetUser();
            var home = new HomePageModel();
            var jsSerializer = new JavaScriptSerializer();
            home = jsSerializer.Deserialize<HomePageModel>(homepage);
            var resultModel = HomePageBll.getHomePageByTypeAndProductId(home.TypeHomePageId, home.ProductId);

            return Json(new { resultCode = resultModel.Code, message = resultModel.Message, data = resultModel.Data }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult IncreaseIOrder(long homePageId)
        {
            var user = UserBll.GetUser();
            var resultModel = HomePageBll.IncreateIOrder(user, homePageId);
            ViewBag.ResultModel = resultModel;

            return Json(new { resultCode = resultModel.Code, message = resultModel.Message}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DecreaseIOrder(long homePageId)
        {
            var user = UserBll.GetUser();
            var resultModel = HomePageBll.DecreaseIOrder(user, homePageId);
            ViewBag.ResultModel = resultModel;

            return Json(new { resultCode = resultModel.Code, message = resultModel.Message}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Featured()
        {
            var user = UserBll.GetUser();
            var resultModel = HomePageBll.getHomePageByStatus((int)StatusEnum.Active, TypeHomePageEnum.Featured);
            ViewBag.ResultModel = resultModel;

            return View();
        }

        public ActionResult Lasted()
        {
            var user = UserBll.GetUser();
            var resultModel = HomePageBll.getHomePageByStatus((int)StatusEnum.Active, TypeHomePageEnum.Latest);
            ViewBag.ResultModel = resultModel;

            return View();
        }

        public ActionResult Specials()
        {
            var user = UserBll.GetUser();
            var resultModel = HomePageBll.getHomePageByStatus((int)StatusEnum.Active, TypeHomePageEnum.Specials);
            ViewBag.ResultModel = resultModel;

            return View();
        }

        public ActionResult BestSellers()
        {
            var user = UserBll.GetUser();
            var resultModel = HomePageBll.getHomePageByStatus((int)StatusEnum.Active, TypeHomePageEnum.Bestsellers);
            ViewBag.ResultModel = resultModel;

            return View();
        }
    }
}
