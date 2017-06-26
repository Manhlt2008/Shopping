using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Core;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.Category;

namespace WebApplication.Lib.Bll
{
    public static class CategoryBll
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public enum CategoryMenuSectionItems
        {
            DealToDay, BestSeller, Newest
        }

        public static List<Category> FindAllRootCategories()
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    return dbContext.Categories.Where(m => m.ParentCategoryId == null && m.Status == StatusEnum.Active).ToList();
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message, exception);
            }
            return Enumerable.Empty<Category>().ToList();
        }

        public static Category Create(string name, long parentCategoryId, long categoryId = 0)
        {
            var catategory = new Category
            {
                Name = name.Trim(),
                ParentCategoryId = parentCategoryId == 0 ? (long?)null : parentCategoryId,
                Status = StatusEnum.Active
            };
            try
            {
                using (var dbContext = new Entities())
                {
                    #region [Validate]

                    if (name.Trim().Equals(string.Empty))
                    {
                        Log.Info("Name cannot be empty");
                        return null;
                    }

                    var cat = dbContext.Categories.FirstOrDefault(m => m.Name.Trim().ToLower().Equals(name.Trim().ToLower()) && m.Status == StatusEnum.Active);
                    if (cat != null)
                    {
                        Log.Info("Name cannot be duplicated");
                        return null;
                    }

                    if (parentCategoryId != 0)
                    {
                        cat = dbContext.Categories.FirstOrDefault(m => m.Id == parentCategoryId);
                        if (cat == null)
                        {
                            Log.Info(string.Format("Parent Category Id {0} not found", parentCategoryId));
                            return null;
                        }
                    }

                    #endregion

                    dbContext.Categories.Add(catategory);

                    if (dbContext.SaveChanges() > 0)
                    {
                        return catategory;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("Create", exception);
            }
            return null;
        }

        public static bool Update(string name, long parentCategoryId, long categoryId = 0)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var category = dbContext.Categories.FirstOrDefault(m => m.Id == categoryId);
                    if (category == null)
                    {
                        Log.Info("Category not found.");
                        return false;
                    }

                    #region [Validate]

                    if (name.Trim().Equals(string.Empty))
                    {
                        Log.Info("Name cannot be empty");
                        return false;
                    }

                    var cat = dbContext.Categories.FirstOrDefault(m => m.Name.Trim().ToLower().Equals(name.Trim().ToLower()) && m.Id != categoryId);
                    if (cat != null)
                    {
                        Log.Info("Name cannot be duplicated");
                        return false;
                    }

                    if (parentCategoryId != 0)
                    {
                        cat = dbContext.Categories.FirstOrDefault(m => m.Id == parentCategoryId);
                        if (cat == null)
                        {
                            Log.Info(string.Format("Parent Category Id {0} not found", parentCategoryId));
                            return false;
                        }
                    }

                    #endregion

                    category.Name = name.Trim();
                    if (parentCategoryId != 0)
                    {
                        category.ParentCategoryId = parentCategoryId;
                    }

                    if (dbContext.SaveChanges() > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("Update", exception);
            }
            return false;
        }

        public static List<Category> FindAllCategories(long? id = null)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    if (id == null)
                    {
                        return
                            dbContext.Categories.Where(
                                m => m.Status == StatusEnum.Active && m.ParentCategoryId.Equals(null))
                                .Include(m => m.Category1)
                                .Include(m => m.Category2)
                                .OrderBy(m => m.Position)
                                .ToList();
                    }

                    return dbContext.Categories.Where(
                                m => m.Id == id && m.Status == StatusEnum.Active && m.ParentCategoryId.Equals(null))
                                .Include(m => m.Category1)
                                .Include(m => m.Category2)
                                .OrderBy(m => m.Position)
                                .ToList();
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindAllCategories", exception);
            }
            return Enumerable.Empty<Category>().ToList();
        }

        #region [Validators]

        public static bool IsNameDuplicated(long categoryId, string name)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    name = name.Trim().ToLower();
                    var category = dbContext.Categories.FirstOrDefault(m => m.Name.Trim().ToLower().Equals(name) && m.Id != categoryId && m.Status == StatusEnum.Active);
                    return category != null;
                }
            }
            catch (Exception exception)
            {
                Log.Error("IsNameDuplicated", exception);
            }
            return false;
        }
        #endregion

        public static Category FindById(long id)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    return
                        dbContext.Categories.Where(m => m.Id == id && m.Status == StatusEnum.Active)
                            .Include(m => m.Category1)
                            .Include(m => m.Category2)
                            .FirstOrDefault();
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindById", exception);
            }
            return null;
        }

        public static void UpdateStatus(long id, int status)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var category = dbContext.Categories.FirstOrDefault(m => m.Id == id);
                    if (category == null)
                    {
                        Log.Info(string.Format("Category with id {0} not found.", id));
                        return;
                    }

                    category.Status = status;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                Log.Error("UpdateStatus", exception);
            }
        }

        public static List<Category> FindAllBySupplierAccountId(long accountId)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var supplierId = dbContext.SupplierAccounts.First(m => m.AccountId == accountId).SupplierId;

                    return
                        dbContext.SupplierCategories.Where(
                            m => m.SupplierId == supplierId && m.Status == StatusEnum.Active)
                            .Select(m => m.Category)
                            .Include(m => m.Category1)
                            .Include(m => m.Category2)
                            .ToList();
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindAllBySupplierId", exception);
            }
            return Enumerable.Empty<Category>().ToList();
        }
        public static List<Category> FindAllBySupplierId(long supplierId)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    return
                        dbContext.SupplierCategories.Where(
                            m => m.SupplierId == supplierId && m.Status == StatusEnum.Active)
                            .Select(m => m.Category)
                            .Include(m => m.Category1)
                            .Include(m => m.Category2)
                            .ToList();
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindAllBySupplierId", exception);
            }
            return Enumerable.Empty<Category>().ToList();
        }

        public static Category FindDealToDayCategory()
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var category =
                        dbContext.Categories.Where(m => m.Status == StatusEnum.Active && m.Type == CategoryTypeEnum.DealToDay)
                            .Include(m => m.Category1)
                            .Include(m => m.Category2)
                            .FirstOrDefault();

                    if (category == null)
                    {
                        category = new Category
                        {
                            Status = StatusEnum.Active,
                            Type = CategoryTypeEnum.DealToDay,
                            Name = "Deal To Day"
                        };

                        dbContext.Categories.Add(category);
                        dbContext.SaveChanges();
                    }

                    return category;
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindDealToDayCategory", exception);
            }
            return null;
        }

        public static int ProductCounter(long categoryId)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var stored = dbContext.FindCategoryPath(categoryId);

                    var categoryIds =
                        (from findCategoryPathResult in stored
                         where findCategoryPathResult.Id != null
                         select findCategoryPathResult.Id.Value).ToList();

                    return dbContext.Products.Count(m => categoryIds.Contains(m.CategoryId) && m.Status == StatusEnum.Active);
                }
            }
            catch (Exception exception)
            {
                Log.Error("ProductCounter()", exception);
            }

            return 0;
        }

        public static Dictionary<CategoryMenuSectionItems, CategoryManageList> InitCategoryForMenuSection()
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var dic = new Dictionary<CategoryMenuSectionItems, CategoryManageList>();

                    #region [DealToDay]
                    var supplier = dbContext.Suppliers.First(m => m.Name.Equals("Deal To Day"));

                    var dealToDayRootCategory =
                        dbContext.SupplierCategories.Where(
                            m =>
                                m.SupplierId == supplier.Id && m.Status == StatusEnum.Active &&
                                m.Category.ParentCategoryId.Equals(null) && m.Category.Status == StatusEnum.Active &&
                                m.Category.Type == CategoryTypeEnum.DealToDay)
                            .Select(m => m.Category)
                            .Include(m => m.Category1)
                            .Include(m => m.Category2)
                            .First();

                    var dealToDayCategoryManageList = new CategoryManageList(dealToDayRootCategory);

                    dic.Add(CategoryMenuSectionItems.DealToDay, dealToDayCategoryManageList);
                    #endregion

                    #region [Newest and BestSeller]

                    var normalCategories =
                        dbContext.Categories.Where(
                            m =>
                                m.Type == CategoryTypeEnum.Normal && m.ParentCategoryId.Equals(null) &&
                                m.Status == StatusEnum.Active)
                            .Include(m => m.Category1)
                            .Include(m => m.Category2)
                            .ToList();

                    var abstractCategory = new Category
                    {
                        Id = long.MaxValue,
                        Name = string.Empty,
                        Category1 = normalCategories
                    };

                    var normalCategoryManageList = new CategoryManageList(abstractCategory);
                    
                    dic.Add(CategoryMenuSectionItems.BestSeller, normalCategoryManageList);
                    dic.Add(CategoryMenuSectionItems.Newest, normalCategoryManageList);

                    #endregion

                    return dic;
                }
            }
            catch (Exception exception)
            {
                Log.Error("InitCategoryForMenuSection()", exception);
            }

            return null;
        }
    }
}