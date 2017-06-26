using System;
using System.Web;
using System.Web.Mvc;
using WebApplication.Filters;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.Slider;
using WebApplication.Models.User;

namespace WebApplication.Controllers
{
    [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
    public class SliderController : Controller
    {
        //
        // GET: /Slider/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Slider()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SliderActive()
        {
            var resultModelActive = new ResultModel();
            resultModelActive = SliderBll.getSliderOrBannerByTypeAndStatus(TypeHomePageEnum.Slider, StatusEnum.Active);
            ViewBag.ResultModelActive = resultModelActive;

            return PartialView("TablePartial/tblListSliderAcitve");
        }

        [HttpPost]
        public ActionResult SliderInActive()
        {
            var resultModelInActive = new ResultModel();
            resultModelInActive = SliderBll.getSliderOrBannerByTypeAndStatus(TypeHomePageEnum.Slider, StatusEnum.InActive);
            ViewBag.ResultModelInActive = resultModelInActive;

            return PartialView("TablePartial/tblListSliderInActive");
        }

        public ActionResult Banner()
        {

            return View();
        }

        [HttpPost]
        public ActionResult BannerActive()
        {
            var resultModelActive = new ResultModel();
            resultModelActive = SliderBll.getSliderOrBannerByTypeAndStatus(TypeHomePageEnum.Banner, StatusEnum.Active);
            ViewBag.ResultModelActive = resultModelActive;

            return PartialView("TablePartial/tblListSliderAcitve");
        }

        [HttpPost]
        public ActionResult BannerInActive()
        {
            var resultModelInActive = new ResultModel();
            resultModelInActive = SliderBll.getSliderOrBannerByTypeAndStatus(TypeHomePageEnum.Banner, StatusEnum.InActive);
            ViewBag.ResultModelInActive = resultModelInActive;

            return PartialView("TablePartial/tblListSliderInActive");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(SliderModel slider)
        {
            ResultModel resultModel = SliderBll.Create(slider);
            if (resultModel.Code != Result.SUCCESS.Code)
            {
                throw new HttpException(500, "Save slider error");
            }

            if (TypeHomePageEnum.Banner.Equals(slider.Type.Trim()))
                return RedirectToAction("Banner", "Slider");
            else
                return RedirectToAction("Slider", "Slider");

            //return Json(new { resultCode = resultModel.Code, message = resultModel.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateStatusSlider(long sliderId, int status)
        {
            var resultModel = new ResultModel();
            var user = UserBll.GetUser();

            resultModel = SliderBll.UpdateStatusSlider(user, sliderId, status);

            return Json(new { resultCode = resultModel.Code, message = resultModel.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult IncreaseIOrder(long sliderId)
        {
            var user = UserBll.GetUser();
            var resultModel = SliderBll.IncreateIOrder(user, sliderId);
            ViewBag.ResultModel = resultModel;

            return Json(new { resultCode = resultModel.Code, message = resultModel.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DecreaseIOrder(long sliderId)
        {
            var user = UserBll.GetUser();
            var resultModel = SliderBll.DecreaseIOrder(user, sliderId);
            ViewBag.ResultModel = resultModel;

            return Json(new { resultCode = resultModel.Code, message = resultModel.Message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(long id)
        {
            ViewBag.Slider = SliderBll.getSliderById(id);

            return View();
        }

        [HttpPost]
        public ActionResult Update(SliderModel sliderModel)
        {
            var user = UserBll.GetUser();
            var resultModel = new ResultModel();

            resultModel = SliderBll.UpdateSlider(user, sliderModel);
            return RedirectToAction("Update");
        }
    }
}
