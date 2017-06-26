using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Filters;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util.Constant;
using WebApplication.Lib.Util.Theme;
using WebApplication.Models.Category;
using WebApplication.Models.Product;
using WebApplication.Models.Stand;
using WebApplication.Models.User;

namespace WebApplication.Controllers
{
    public class ProductController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin, RoleEnum.SupplierManager)]
        public ActionResult Manage(long catId = 0, string sort = "All", int order = 1, int limit = 5, int page = 1, string query = "", long? supplierId = null)
        {
            var user = UserBll.GetUser();

            if (user.RoleId != RoleEnum.Admin && user.RoleId != RoleEnum.Manager)
            {
                if (supplierId == null)
                {
                    supplierId = SupplierBll.FindOneDefaultSupplierIdByAccountId(user.Id);

                    if (supplierId == long.MinValue)
                    {
                        throw new HttpException(403, "Access Denied");
                    }
                }
            }

            if (supplierId != null)
            {
                //todo
                ViewBag.FindAllProductModel = ProductBll.FindAllProduct(catId, sort, order, limit, page, query, StatusEnum.Active, supplierId.Value);
                var Id = supplierId.Value;
                var supplier = SupplierBll.FindOneById(supplierId.Value);
                ViewBag.Categories = CategoryBll.FindAllBySupplierId(Id).Select(m => new CategoryManageList(m)).ToList();
                ViewBag.SupplierProduct = SupplierBll.FindOneById(Id);
            }
            else
            {
                ViewBag.FindAllProductModel = ProductBll.FindAllProduct(catId, sort, order, limit, page, query);
                ViewBag.Categories = CategoryBll.FindAllCategories().Select(m => new CategoryManageList(m)).ToList();
            }
            ViewBag.ActionName = TempData["ActionName"];
            ViewBag.ControllerName = TempData["ControllerName"];
            ViewBag.CategoryId = catId;
            ViewBag.UpdateProductStatus = TempData["UpdateProductStatus"];
            ViewBag.SearchString = query;
            return View();
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        public ActionResult PendingProduct(long catId = 0, string sort = "All", int order = 1, int limit = 5, int page = 1)
        {
            var user = UserBll.GetUser();
            if (user.RoleId == RoleEnum.SupplierManager || user.RoleId == RoleEnum.SupplierEmployee)
            {
                var supplierProducts = SupplierBll.ListSupplierByAccount(user.Id);
                var findAllProductModel = new List<FindAllProductModel>();
                foreach (var supplierProduct in supplierProducts)
                {
                    var productModel = ProductBll.FindAllProduct(catId, sort, order, limit, page, "", StatusEnum.Pending, supplierProduct.Id);
                    findAllProductModel.Add(productModel);
                }
                ViewBag.FindAllProductModel = findAllProductModel;
            }
            else
            {
                ViewBag.FindAllProductModel = ProductBll.FindAllProduct(catId, sort, order, limit, page, "", StatusEnum.Pending);
            }

            ViewBag.CategoryId = catId;
            var actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            var controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            TempData["ActionName"] = actionName;
            TempData["ControllerName"] = controllerName;
            ViewBag.Categories = CategoryBll.FindAllCategories().Select(m => new CategoryManageList(m)).ToList();
            ViewBag.UpdateProductStatus = TempData["UpdateProductStatus"];
            return View("../Product/Manage");
        }
        #region [View]

        public ActionResult View(long id)
        {
            var product = ProductBll.FindById(id);

            if (product == null)
            {
                throw new HttpException(404, "Product not found.");
            }

            product.Category = CategoryBll.FindById(product.CategoryId);

            ViewBag.Product = new ProductViewModel(product);
            ViewBag.Stand = ProductBll.GetStandByProductId(product.Id);
            var relatedProduct = ProductBll.FindAllRelatedProduct(id);
            var relatedProductThumbnails = relatedProduct.Select(p => new ProductThumbnailModel(p)).ToList();
            ViewBag.RelatedProduct = relatedProductThumbnails;

            var categories = CategoryBll.FindAllCategories();
            ViewBag.Categories = categories.Select(cat => new CategoryManageList(cat)).ToList();

            return View(ThemeName.GetView(ThemeName.ViewName.ProductViewNameEnum.View));
        }
        #endregion

        #region [Create]
        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin, RoleEnum.SupplierManager)]
        public ActionResult Create(long? supplierId = null)
        {
            var user = UserBll.GetUser();
            if (supplierId != null)
            {
                ViewBag.SupplierId = supplierId.Value;
            }
            if (user.RoleId == RoleEnum.Admin || user.RoleId == RoleEnum.Manager)
            {
                if (supplierId != null)
                {
                    ViewBag.Categories = CategoryBll.FindAllBySupplierId(supplierId.Value).Select(m => new CategoryManageList(m)).ToList();
                }
                else
                {
                    ViewBag.Categories = CategoryBll.FindAllCategories().Select(m => new CategoryManageList(m)).ToList();
                }
            }
            else
            {
                ViewBag.Categories = CategoryBll.FindAllBySupplierAccountId(user.Id).Select(m => new CategoryManageList(m)).ToList();
            }

            return View();
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin, RoleEnum.SupplierManager)]
        [HttpPost]
        public ActionResult Create(ProductModel product)
        {
            var isSaveSuccess = false;
            if (product.Id == -1)
            {
                // Create New Prodcut
                var supplier = SupplierBll.FindOneById(product.SupplierId);

                // TODO: You cannot check supplier null due to admin is using same function. - Henry.
                //if (supplier == null)
                //{
                //    throw new HttpException(403, "You are not supplier.");
                //}

                var user = UserBll.GetUser();
                if (user.RoleId == RoleEnum.Admin || user.RoleId == RoleEnum.Manager)
                {
                    product.Status = StatusEnum.Active;
                }
                else
                {
                    product.Status = StatusEnum.Pending;
                }

                isSaveSuccess = ProductBll.Create(product);
                if (!isSaveSuccess)
                {
                    throw new HttpException(500, "Save product error");
                }

                if (supplier != null)
                {
                    return RedirectToAction("ManageProduct", "Supplier");
                }

                return RedirectToAction("Manage", "Product");
            }

            isSaveSuccess = ProductBll.Update(product);

            TempData["UpdateProductStatus"] = isSaveSuccess;

            return RedirectToAction("Update", new { id = product.Id });
        }
        #endregion

        #region [Update]
        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin, RoleEnum.SupplierEmployee, RoleEnum.SupplierManager)]
        public ActionResult Update(long id, long? supplierId = null)
        {
            ViewBag.Categories = CategoryBll.FindAllCategories().Select(m => new CategoryManageList(m)).ToList();

            var product = ProductBll.FindById((long?)TempData["ProductId"] ?? id);
            if (product == null)
            {
                throw new HttpException(404, "Product not found");
            }

            var user = UserBll.GetUser();
            if (!ProductBll.CheckEditableAccount(user.Id, id, user.RoleId))
            {
                throw new HttpException(404, "Product not found");
            }

            if (supplierId != null)
            {
                ViewBag.Product = ProductBll.GetNewUpdateProduct(id, supplierId.Value);
            }
            else
            {
                ViewBag.Product = new ProductModel(product);
            }

            ViewBag.UpdateProductStatus = TempData["UpdateProductStatus"];

            return View("Create");
        }
        #endregion

        #region [Category]

        public ActionResult Category(long id = 0, string sort = "default", int order = 1, int limit = 6, int page = 1, string query = "")
        {
            ViewBag.FindAllProductModel = ProductBll.FindAllProduct(id, sort, order, limit, page, query);
            ViewBag.ActionName = "Category";
            ViewBag.CategoryName = "Product";
            return View(ThemeName.GetView(ThemeName.ViewName.ProductViewNameEnum.Category));
        }
        #endregion

        #region [Validators]
        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin, RoleEnum.SupplierManager)]
        [HttpPost]
        public JsonResult IsNameDuplicated(string name, long productId = 0)
        {
            if (productId == -1)
            {
                productId = 0;
            }
            var isDuplicated = ProductBll.IsNameDuplicated(name, productId);
            return Json(new { isDuplicated = isDuplicated });
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        [HttpPost]
        public JsonResult IsAllowWriteComment(string code, long productId)
        {
            var user = UserBll.GetUser();

            if (user == null)
            {
                throw new HttpException(403, "Access Denied");
            }

            var order = OrderBll.FindOneByOrderCode(code.Trim().ToLower(), user.Id, productId);
            return Json(new { IsAllow = order != null });
        }
        #endregion

        #region [Delete]
        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin, RoleEnum.SupplierManager, RoleEnum.SupplierEmployee)]
        public ActionResult Delete(long id, long catId = 0, string sort = "All", int order = 1, int limit = 5, int page = 1, long? supplierId = null)
        {
            var user = UserBll.GetUser();

            var isSuccess = ProductBll.Delete(id, user.RoleId);

            if (!isSuccess)
            {
                throw new HttpException(403, "Access Denied.");
            }
            if (supplierId != null)
            {
                return RedirectToAction("Manage", "Product", new
                {
                    catId = catId,
                    sort = sort,
                    order = order,
                    limit = limit,
                    page = page,
                    supplierId = supplierId
                });
            }
            return RedirectToAction("Manage", "Product", new
            {
                catId = catId,
                sort = sort,
                order = order,
                limit = limit,
                page = page
            });
        }
        #endregion

        #region [Approve]
        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        public ActionResult Approve(long id, string currentcontroller, long catId = 0, string sort = "All", int order = 1, int limit = 5, int page = 1)
        {
            var isSuccess = ProductBll.Approve(id);

            if (!isSuccess)
            {
                throw new HttpException(403, "Access Denied.");
            }
            TempData["UpdateProductStatus"] = isSuccess;

            if (currentcontroller.Equals("Manage"))
            {
                return RedirectToAction("Manage", "Product", new
                {
                    catId = catId,
                    sort = sort,
                    order = order,
                    limit = limit,
                    page = page
                });
            }
            return RedirectToAction("PendingProduct", "Product", new
            {
                catId = catId,
                sort = sort,
                order = order,
                limit = limit,
                page = page
            });
        }
        #endregion

        #region [Update Quantity]
        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        public ActionResult UpdateQuantity(long id, int quantity)
        {
            var result = new ResultModel();
            var user = UserBll.GetUser();
            result = ProductBll.UpdateQuantity(user, id, quantity);

            return Json(new { resultCode = result.Code, message = result.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region [Find All By Ids]

        [HttpPost]
        public JsonResult FindAllByIds(List<long> ids)
        {
            var products = ProductBll.FindAllByIds(ids);

            var data = new List<object>();

            foreach (var product in products)
            {
                var coverImg = product.Images.FirstOrDefault(m => m.Type == ImageTypeEnum.Cover);
                var cover = coverImg == null ? -1 : coverImg.Id;

                data.Add(new
                {
                    Id = product.Id,
                    Name = product.Name,
                    Quantity = product.Quantity,
                    Cover = cover,
                    Price = product.Price,
                    Gallery = product.Images.Where(m => m.Type == ImageTypeEnum.Gallery).Select(m => m.Id).ToList(),
                    IsFeatured = product.IsFeatured,
                    Category = new
                    {
                        Id = product.Category == null ? -1 : product.Category.Id,
                        Name = product.Category == null ? string.Empty : product.Category.Name
                    }
                });

            }

            return Json(data);
        }
        #endregion


        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        [HttpPost]
        public JsonResult WriteComment(ProductReviewModel model)
        {
            var user = UserBll.GetUser();

            if (user == null)
            {
                throw new HttpException(403, "Access Denied");
            }

            var order = OrderBll.FindOneByOrderCode(model.Code.Trim().ToLower(), user.Id, model.ProductId);

            if (order == null)
            {
                throw new HttpException(404, "Required order code not found.");
            }

            var review = ReviewBll.WriteReview(order, model);

            if (review == null)
            {
                return Json(new { Status = false });
            }

            return Json(new
            {
                Status = true,
                ReviewId = review.Id
            });
        }

        [HttpPost]
        public JsonResult FindAllByProductName(string query)
        {
            var products = ProductBll.FindAllByProductName(query);

            var data = new List<object>();

            foreach (var product in products)
            {
                data.Add(new
                {
                    Id = product.Id,
                    Name = product.Name,
                    Quantity = product.Quantity,
                    Price = product.Price,
                    IsFeatured = product.IsFeatured,
                    Category = new
                    {
                        Id = product.Category == null ? -1 : product.Category.Id,
                        Name = product.Category == null ? string.Empty : product.Category.Name
                    }
                });

            }

            return Json(data);
        }

        public ActionResult Newest(long id = 0, string sort = "default", int order = 1, int limit = 6, int page = 1)
        {
            ViewBag.FindAllProductModel = ProductBll.FindAllHomeProduct(id, sort, order, limit, page, TypeHomePageEnum.Latest);

            ViewBag.IsNewestPage = true;
            ViewBag.ActionName = "Newest";
            ViewBag.CategoryName = "Product";
            return View(ThemeName.GetView(ThemeName.ViewName.ProductViewNameEnum.Category));
        }

        public ActionResult BestSeller(long id = 0, string sort = "default", int order = 1, int limit = 6, int page = 1)
        {
            ViewBag.FindAllProductModel = ProductBll.FindAllHomeProduct(id, sort, order, limit, page, TypeHomePageEnum.Bestsellers);

            ViewBag.IsNewestPage = true;
            ViewBag.ActionName = "BestSeller";
            ViewBag.CategoryName = "Product";
            return View(ThemeName.GetView(ThemeName.ViewName.ProductViewNameEnum.Category));
        }
        public ActionResult Stand(long standId = 0, string sort = "default", int order = 1, int limit = 6, int page = 1)
        {
            ViewBag.FindAllProductStand = ProductBll.FindAllStandProduct(standId, sort, order, limit, page);
            ViewBag.ActionName = "Stand";
            ViewBag.CategoryName = "Product";
            return View(ThemeName.GetView(ThemeName.ViewName.ProductViewNameEnum.Stand));
        }
    }
}
