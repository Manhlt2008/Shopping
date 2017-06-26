using System.Linq;
using System.Web.Mvc;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Constant;
using WebApplication.Lib.Util.Theme;
using WebApplication.Models.Category;
using WebApplication.Models.Product;
using WebApplication.Models.User;

namespace WebApplication.Controllers
{

    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            // Featured [Center]
            var featuredProduct = ProductBll.FindAllHomePageProducts(TypeHomePageEnum.Featured);
            var featuredProductThumbnails = featuredProduct.Select(product => new ProductThumbnailModel(product)).ToList();
            // Lasted [Center]
            var latestProduct = ProductBll.FindAllHomePageProducts(TypeHomePageEnum.Latest);
            var latestProductThumbnails = latestProduct.Select(product => new ProductThumbnailModel(product)).ToList();

            //Slider 
            var resultModelSliderActive = new ResultModel();
            resultModelSliderActive = SliderBll.getSliderOrBannerByTypeAndStatus(TypeHomePageEnum.Slider, StatusEnum.Active);

            //Banner
            var resultModelBannerActive = new ResultModel();
            resultModelBannerActive = SliderBll.getSliderOrBannerByTypeAndStatus(TypeHomePageEnum.Banner, StatusEnum.Active);

            // Categories
            var categories = CategoryBll.FindAllCategories();

            ViewBag.FeaturedProduct = featuredProductThumbnails;
            ViewBag.LatestProduct = latestProductThumbnails;

            ViewBag.BannerActive = resultModelBannerActive;
            ViewBag.SliderActive = resultModelSliderActive;

            ViewBag.Categories = categories.Select(cat => new CategoryManageList(cat)).ToList();

            return View(ThemeName.GetView(ThemeName.ViewName.HomeViewNameEnum.Index));
        }
    }
}
