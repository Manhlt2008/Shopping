using System.Web.Mvc;
using WebApplication.Filters;
using WebApplication.Lib.Bll.ApiHelper.DealToDay;
using WebApplication.Lib.Util.Constant;

namespace WebApplication.Controllers
{
    [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
    public class DealToDayController : Controller
    {
        //
        // GET: /DealToDay/

        public ActionResult Index()
        {
            ViewBag.DealToDayNewDeals = DealToDayBll.GetAllEDeal();
            ViewBag.IsParseComplete = TempData["IsParseComplete"];
            return View();
        }

        public ActionResult Approve(int dealId)
        {
            var product = DealToDayBll.AddByDealId(dealId);

            if (product == null)
            {
                TempData["IsParseComplete"] = false;
                return RedirectToAction("Index");
            }

            TempData["ProductId"] = product.Id;
            return RedirectToAction("Update", "Product", new {id = product.Id});
        }

        // Henry: DONOT Remove
//        public string FetchAll()
//        {
//            var deals = DealToDayBll.GetAllEDeal();
//            Debug.WriteLine("Begin fetch");
//
//            var count = 1;
//            var beginTime = DateTime.Now;
//            foreach (var deal in deals.result)
//            {
//                Debug.WriteLine(count++ + "Fetching " + deal.dealId + " - " + deal.dealName);
//                DealToDayBll.AddByDealId(deal.dealId);
//            }
//
//            var endTime = DateTime.Now;
//
//            Debug.WriteLine("Total: " + endTime.Subtract(beginTime));
//
//            return "Total: " + endTime.Subtract(beginTime);
//        }
    }
}
