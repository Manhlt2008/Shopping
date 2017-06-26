using System.Activities.Statements;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using WebApplication.Filters;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.Category;
using WebApplication.Models.Product;
using Supplier = WebApplication.Models.Supplier.Supplier;

namespace WebApplication.Controllers
{
    public class SupplierController : Controller
    {
        //
        // GET: /Supplier/
        private void FindAllSupplier(string sort = "default", int order = 1, int limit = 6, int page = 1, string query = "")
        {
            query = query.Trim();

            //var categories = CategoryBll.FindAllCategories();
            //var category = CategoryBll.FindById(categoryId);
            var suppliers = SupplierBll.FindAllBySupplierName(query);

            ViewBag.Supplier = suppliers.ToList();
            ViewBag.Query = query;
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin, RoleEnum.SupplierManager)]
        public ActionResult Manage(string sort = "All", int order = 1, int limit = 5, int page = 1)
        {
            var user = UserBll.GetUser();
            if (user.RoleId == RoleEnum.Manager || user.RoleId == RoleEnum.Admin)
            {

                ViewBag.Suppliers = SupplierBll.FindAllBySupplierName("").Where(m=>m.Status == StatusEnum.Active).ToList();
            }
            else
            {
                ViewBag.Suppliers = SupplierBll.ListSupplierByAccount(user.Id);
            }

            ViewBag.UpdateSupplierStatus = TempData["UpdateSupplierStatus"];
            ViewBag.Categories = CategoryBll.FindAllCategories().Select(m => new CategoryManageList(m)).ToList();
            return View();
        }
        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin, RoleEnum.SupplierManager)]
        public ActionResult Create()
        {
            ViewBag.Categories = CategoryBll.FindAllCategories().Select(m => new CategoryManageList(m)).ToList();
            ViewBag.Accounts = AccountBll.FindAllAccount();
            ViewBag.SelectedAccounts = AccountBll.FindAllAccountBySupplier();
            return View();
        }
        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin, RoleEnum.SupplierManager)]
        [HttpPost]
        public ActionResult Create(Models.Supplier.Supplier model)
        {
            var ListSuppliers = SupplierBll.FindAllBySupplierName("");
            var isExisted = ListSuppliers.Any(m => m.Email == model.Email);
            if (isExisted == true)
            {
                ModelState.AddModelError("", "Email đã tồn tại, quý khách vui lòng sử dụng email khác ");
                return View(model);
            }
            else
            {
                var isSuccess = SupplierBll.Create(model);
                TempData["IsSuccess"] = isSuccess;
                if (isSuccess)
                {
                    return RedirectToAction("Manage", "Supplier");
                }
                else
                {
                    ModelState.AddModelError("", "Đã có lỗi xảy ra, vui lòng thử lại sau!");
                    return View(model);
                }
            }
        }
        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin, RoleEnum.SupplierManager)]
        public ActionResult CreateProduct()
        {
            var user = UserBll.GetUser();
            ViewBag.Categories = CategoryBll.FindAllBySupplierAccountId(user.Id).Select(m => new CategoryManageList(m)).ToList();
            return View("../Product/Create");
        }
        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin, RoleEnum.SupplierManager)]
        public ActionResult ManageProduct(long catId = 0, string sort = "All", int order = 1, int limit = 5, int page = 1)
        {
            var supplier = SupplierBll.GetSupplierByUserSession();

            // TODO: Check is this ViewBag used on Product/Manage
            ViewBag.Products = ProductBll.FindAllBySupplierId(supplier.Id).Select(m => new ProductThumbnailModel(m)).ToList();

            ViewBag.FindAllProductModel = ProductBll.FindAllProduct(catId, sort, order, limit, page, string.Empty, StatusEnum.CombineActivePending, supplier.Id);
            ViewBag.CategoryId = catId;
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            TempData["ActionName"] = actionName;
            TempData["ControllerName"] = controllerName;
            return View("../Product/Manage");
        }
        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin, RoleEnum.SupplierManager)]
        [HttpPost]
        public ActionResult UpdateStatus(string email, byte status)
        {
            var user = UserBll.GetUser();
            var result = SupplierBll.UpdateStatusSupplier(user, email, status);

            return Json(new { resultCode = result.Code, message = result.Message }, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin, RoleEnum.SupplierManager)]
        public ActionResult UpdateInfoSupplier(long id)
        {
            ViewBag.Categories = CategoryBll.FindAllCategories().Select(m => new CategoryManageList(m)).ToList();
            var supplier = SupplierBll.FindOneById(id);
            var ListSuppliers = SupplierBll.FindAllBySupplierName("");
            Supplier supplierModel = new Supplier
            {
                Name = supplier.Name,
                Address = supplier.Address,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Website = supplier.Website,
                Facebook = supplier.Facebook,
                CategoryIds = supplier.SupplierCategories.Where(m => m.Status == StatusEnum.Active).Select(m => m.CategoryId).ToList(),
                Categories = new CategoryDropDownListModel(supplier.SupplierCategories.Select(m => m.Category).Where(m => m.Status == StatusEnum.Active).ToList())
            };
            ViewBag.Supplier = supplierModel;
            ViewBag.SelectedAccounts = AccountBll.FindAllAccountBySupplier(id);
            ViewBag.Accounts = AccountBll.FindAllAccount();
            return View("Create");
        }
    }
}
