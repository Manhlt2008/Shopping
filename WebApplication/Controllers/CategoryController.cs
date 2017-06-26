using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Filters;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.Category;

namespace WebApplication.Controllers
{
    public class CategoryController : Controller
    {

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");

        }

        #region [Manage]
        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        public ActionResult Manage(long? id)
        {
            var categories = CategoryBll.FindAllCategories(id);

            var list = categories.Select(category => new CategoryManageList(category)).ToList();

            if (list.Count == 1)
            {
                ViewBag.CategoryInfo = list.First();
                list = list.First().CategoryManageLists;
            }
            ViewBag.CategoryList = list;
            ViewBag.CategoryId = id;
            return View();
        }
        #endregion

        #region [Create Category]
        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        public ActionResult Create()
        {
            ViewBag.ParentCategory = new CategoryDropDownListModel(CategoryBll.FindAllRootCategories());
            return View();
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        [HttpPost]
        public ActionResult Create(CategoryModel model)
        {
            CategoryBll.Create(model.Name, model.ParentCategoryId);
            return RedirectToAction("Manage", "Category");
        }
        #endregion

        #region [Update Category]
        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        public ActionResult Update(long id)
        {
            var category = CategoryBll.FindById(id);

            ViewBag.ParentCategory = new CategoryDropDownListModel(CategoryBll.FindAllRootCategories());

            if (category == null)
            {
                throw new HttpException(404, "Category Not Found");
            }

            var model = new CategoryModel(category);
            ViewBag.Category = model;
            return View("Create");
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        [HttpPost]
        public ActionResult Update(CategoryModel model)
        {
            var isSuccess = CategoryBll.Update(model.Name, model.ParentCategoryId, model.Id);
            if (!isSuccess)
            {
                throw new HttpException(500, "Connection Result. Please contact administrator for more information.");
            }

            return RedirectToAction("Manage", "Category");
        }
        #endregion

        #region [Delete Category]

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        public ActionResult Delete(long id)
        {
            CategoryBll.UpdateStatus(id, StatusEnum.InActive);
            return RedirectToAction("Manage", "Category");
        }
        #endregion

        #region [View Category]

        public ActionResult View(long id)
        {
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region [Restful API Validator]
        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        [HttpPost]
        public JsonResult IsNameDuplicated(string name, long categoryId = 0)
        {
            if (name != null)
            {
                var isDuplicated = CategoryBll.IsNameDuplicated(categoryId, name);
                return Json(new { isDuplicated = isDuplicated });
            }

            return Json(new { isDuplicated = true });
        }

        #endregion
    }
}
