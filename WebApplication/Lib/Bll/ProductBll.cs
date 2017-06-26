﻿using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Transactions;
using System.Web;
using System.Web.Script.Serialization;
using WebApplication.Lib.Bll.ApiHelper.DealToDay;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.Category;
using WebApplication.Models.Product;
using WebApplication.Models.Stand;
using WebApplication.Models.User;

namespace WebApplication.Lib.Bll
{
    public static class ProductBll
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public const int FeaturedDefaultLimit = 5;
        public const int LatestDefaultLimit = 5;

        public const string SortDefault = "default";
        public const string SortName = "name";
        public const string SortPrice = "price";
        public const string SortLatest = "latest";

        public const int Asc = 1;
        public const int Desc = 0;

        #region [Validation]
        public static bool IsNameDuplicated(string name, long productId = 0)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    name = name.Trim().ToLower();
                    var category =
                        dbContext.Products.FirstOrDefault(
                            m =>
                                m.Name.Trim().ToLower().Equals(name) && m.Id != productId &&
                                m.Status == StatusEnum.Active);
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

        #region [Create]

        public static bool Create(ProductModel model)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        var user = UserBll.GetUser();
                        var createdDate = DateTime.Now;
                        var count = 1;
                        var product = new Product
                        {
                            CategoryId = model.CategoryId,
                            Description = model.Description,
                            Name = model.Name,
                            Price = model.Price,
                            IsFeatured = model.IsFeatured,
                            Quantity = model.Quantity,
                            CreatedDate = createdDate,
                            Status = model.Status,
                            ShortDescription = model.ShortDescription
                        };

                        #region [Validation]
                        // Check is supplier validate
                        if (model.SupplierId > 0)
                        {
                            var sup = dbContext.Suppliers.FirstOrDefault(m => m.Id == model.SupplierId);
                            if (sup == null)
                            {
                                throw new HttpException(403, "Access Denied");
                            }

                            if (sup.Status == StatusEnum.InActive)
                            {
                                throw new HttpException(403, "Access Denied");
                            }
                        }


                        if (model.Name.Trim().Equals(string.Empty))
                        {
                            Log.Info("Product name cannot be empty.");
                            return false;
                        }

                        if (IsNameDuplicated(model.Name))
                        {
                            Log.Info(string.Format("Product name {0} is duplicated.", model.Name));
                            return false;
                        }

                        if (model.Quantity < 0 || model.Price < 0)
                        {
                            Log.Info(model.Quantity < 0 ? "Quantity cannot be less than 0." : "Price cannot be less than 0.");
                            return false;
                        }
                        #endregion

                        dbContext.Products.Add(product);
                        dbContext.SaveChanges();

                        if (model.Cover == null)
                        {
                            Log.Info("Cover cannot be null.");
                            return false;
                        }
                        var coverImg = ImageBll.Save(model.Cover, product.Id, string.Format("_{0}_{1}", count++, product.Id));
                        if (coverImg == null)
                        {
                            Log.Error("Save image fail.");
                            return false;
                        }

                        coverImg.Type = ImageTypeEnum.Cover;
                        coverImg.CreatedDate = createdDate;
                        dbContext.Images.Add(coverImg);

                        foreach (var file in model.Gallery)
                        {
                            if (file == null)
                            {
                                continue;
                            }

                            var galleryImg = ImageBll.Save(file, product.Id, string.Format("_{0}_{1}", count++, product.Id));

                            if (galleryImg == null)
                            {
                                Log.Error("Save image fail.");
                                return false;
                            }

                            galleryImg.Type = ImageTypeEnum.Gallery;
                            galleryImg.CreatedDate = createdDate;
                            dbContext.Images.Add(galleryImg);
                        }

                        dbContext.SaveChanges();

                        if (model.SupplierId > 0)
                        {
                            var supplier = SupplierBll.FindOneById(model.SupplierId);
                            var supplierCategory = dbContext.SupplierCategories.FirstOrDefault(m => m.CategoryId == model.CategoryId && m.SupplierId == supplier.Id);

                            if (supplierCategory == null)
                            {
                                return false;
                            }
                            var supplierAccount = dbContext.SupplierAccounts.FirstOrDefault(m => m.AccountId == user.Id && m.SupplierId == supplier.Id);
                            var supplierProduct = new SupplierProduct();
                            if (supplierAccount == null)
                            {
                                supplierProduct.ExtraInfo = string.Empty;
                                supplierProduct.ProductId = product.Id;
                                supplierProduct.Status = StatusEnum.Active;
                                supplierProduct.SupplierId = supplier.Id;
                                supplierProduct.SupplierCategoryId = supplierCategory.Id;
                                supplierProduct.CreatedSupplierAccountId = null;
                                supplierProduct.CreatedByAdminId = user.Id;
                            }
                            else
                            {
                                supplierProduct.ExtraInfo = string.Empty;
                                supplierProduct.ProductId = product.Id;
                                supplierProduct.Status = StatusEnum.Active;
                                supplierProduct.SupplierId = supplier.Id;
                                supplierProduct.SupplierCategoryId = supplierCategory.Id;
                                supplierProduct.CreatedSupplierAccountId = supplierAccount.Id;
                            }


                            dbContext.SupplierProducts.Add(supplierProduct);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            var supplierProduct = new SupplierProduct
                            {
                                ExtraInfo = string.Empty,
                                ProductId = product.Id,
                                Status = StatusEnum.Active,
                                SupplierId = null,
                                SupplierCategoryId = null,
                                CreatedSupplierAccountId = null,
                                CreatedByAdminId = user.Id
                            };
                            dbContext.SupplierProducts.Add(supplierProduct);
                            dbContext.SaveChanges();
                        }

                        scope.Complete();

                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("Create", exception);
            }

            return false;
        }

        #endregion

        public static List<Product> FindAllFeaturedProducts(int limit = FeaturedDefaultLimit)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    return
                        dbContext.Products.Where(m => m.IsFeatured && m.Status == StatusEnum.Active && m.Category.Status == StatusEnum.Active)
                            .OrderBy(m => m.CreatedDate)
                            .Take(limit)
                            .Include(m => m.Category)
                            .Include(m => m.Images)
                            .ToList();
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindAllFeaturedProducts", exception);
            }

            return Enumerable.Empty<Product>().ToList();
        }

        public static List<Product> FindAllHomePageProducts(long typeHomePage)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    return
                        dbContext.HomePages.Where(
                            m => m.Status == StatusEnum.Active && m.TypeHomePageId.Equals(typeHomePage) && m.Product.Category.Status == StatusEnum.Active && m.Product.Status == StatusEnum.Active)
                            .Select(m => m.Product)
                            .Include(m => m.Category)
                            .Include(m => m.Images)
                            .ToList();
                }
            }
            catch (Exception exception)
            {
                Log.Error("ListAllLatestProducts", exception);
            }

            return Enumerable.Empty<Product>().ToList();
        }


        public static List<Product> FindByCategoryId(long categoryId, string sort, int order, int limit, int page, string search,
              out int totalProducts, bool isLoadSubCategory = true, int status = StatusEnum.Active, long? supplierId = null)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    #region [Get Category info]
                    var categoryIds = new List<long> { categoryId };

                    if (categoryId != 0)
                    {
                        if (
                            dbContext.Categories.FirstOrDefault(
                                m =>
                                    m.Id == categoryId && m.Status == StatusEnum.Active &&
                                    ((m.ParentCategoryId == null) ||
                                     (m.ParentCategoryId != null && m.Category2.Status == StatusEnum.Active))) == null)
                        {
                            totalProducts = 0;
                            return Enumerable.Empty<Product>().ToList();
                        }
                    }

                    if (categoryId != 0 && isLoadSubCategory)
                    {
                        categoryIds.AddRange(
                            dbContext.Categories.Where(
                                c => c.ParentCategoryId != null && c.ParentCategoryId.Value == categoryId && c.Status == StatusEnum.Active)
                                .Select(m => m.Id)
                                .ToList());
                    }
                    #endregion

                    string orderByQuery;

                    switch (sort)
                    {
                        case SortLatest:
                            orderByQuery = order == Asc
                                ? " [CreatedDate] ASC "
                                : " [CreatedDate] DESC ";
                            break;
                        case SortPrice:
                            orderByQuery = order == Asc
                                ? " [Price] ASC "
                                : " [Price] DESC ";
                            break;
                        default:
                            orderByQuery = order == Asc
                                ? " [Name] ASC "
                                : " [Name] DESC ";
                            break;
                    }
                    var nativeSqlQuery = "";
                    var nativateCountSqlQuery = "";
                    var parameters = new List<object>();
                    var parameters1 = new List<object>();

                    if (supplierId == null)
                    {
                        nativeSqlQuery = string.Format("SELECT [Id], " +
                                        "[CategoryId], " +
                                        "[Name], " +
                                        "[ShortDescription], " +
                                        "[Description], " +
                                        "[Price], " +
                                        "[Quantity], " +
                                        "[IsFeatured], " +
                                        "[CreatedDate], " +
                                        "[Status], [DealToDayInfo] " +
                                        "FROM (" +
                                        "  SELECT ROW_NUMBER() OVER ( ORDER BY {0} ) AS RowNum, * " +
                                        "      FROM Product WHERE " +
                                        "          Name LIKE @query " +
                                        "              {1} " +
                                        "              AND [Status] = {4} ) AS RowConstrainedResult " +
                                        "  WHERE RowNum >= {2} AND RowNum <= {3} ORDER BY RowNum",
                                        orderByQuery,
                                        (categoryId != 0 && isLoadSubCategory) ? " AND CategoryId IN @categoryIds " : "",
                                       (limit == 0 && page == 1 ? 0 : (limit * (page - 1) + 1)), (limit == 0 && page == 1 ? "RowNum" : (limit * page).ToString()), status);

                        nativateCountSqlQuery =
                           string.Format("SELECT COUNT(*) FROM Product WHERE Name LIKE @query {0} AND [Status] = {1} ",
                               (categoryId != 0 && isLoadSubCategory) ? " AND CategoryId IN @categoryIds " : "",
                               status);

                    }
                    else
                    {
                        nativeSqlQuery = string.Format("SELECT [Product].[Id], " +
                                        "[Product].[CategoryId], " +
                                        "[Product].[Name], " +
                                        "[Product].[ShortDescription], " +
                                        "[Product].[Description], " +
                                        "[Product].[Price], " +
                                        "[Product].[Quantity], " +
                                        "[Product].[IsFeatured], " +
                                        "[Product].[CreatedDate], " +
                                        "[Product].[Status], [Product].[DealToDayInfo]" +
                                      "FROM (" +
                                        "  SELECT ROW_NUMBER() OVER ( ORDER BY {0} ) AS RowNum, [Product].* " +
                                        "      FROM ( Product INNER JOIN SupplierProduct ON Product.Id=SupplierProduct.ProductId ) WHERE " +
                                        "          [Product].[Name] LIKE @query " +
                                        "              {1} " +
                                        "              AND [Product].[Status] IN ({4}) AND [SupplierProduct].[SupplierId] = {5} {6}) AS [Product] " +
                                        "  WHERE RowNum >= {2} AND RowNum <= {3} ORDER BY RowNum",
                                        orderByQuery,
                                        (categoryId != 0 && isLoadSubCategory) ? " AND CategoryId IN @categoryIds " : "",
                                        (limit == 0 && page == 1 ? 0 : (limit * (page - 1) + 1)), (limit == 0 && page == 1 ? "RowNum" : (limit * page).ToString()), StatusEnum.GetCombineStatus(status), supplierId, status == StatusEnum.Pending ? "OR ([SupplierProduct].ExtraInfo IS NOT NULL)" : string.Empty);

                        nativateCountSqlQuery =
                           string.Format("SELECT COUNT(*) FROM ( Product INNER JOIN SupplierProduct ON Product.Id=SupplierProduct.ProductId ) WHERE Name LIKE @query {0} AND [Product].[Status] = {1} AND [SupplierProduct].[SupplierId] = {2} " +
                                         "{3}",
                               (categoryId != 0 && isLoadSubCategory) ? " AND CategoryId IN @categoryIds " : "",
                               status, supplierId, status == StatusEnum.Pending ? "OR ([SupplierProduct].ExtraInfo IS NOT NULL)" : string.Empty);
                    }


                    var catIds = "";

                    foreach (var id in categoryIds)
                    {
                        catIds += id;

                        if (categoryIds.IndexOf(id) != (categoryIds.Count - 1))
                        {
                            catIds += ",";
                        }
                    }

                    parameters = new List<object>
                    {
                        new SqlParameter("query", string.Format("%{0}%", StringUtil.RemoveVietnameseTone(
                            StringUtil.RemoveSign4VietnameseString(search.ToLower().Trim()))))
                    };

                    parameters1 = new List<object>
                    {
                        new SqlParameter("query", string.Format("%{0}%", StringUtil.RemoveVietnameseTone(
                            StringUtil.RemoveSign4VietnameseString(search.ToLower().Trim()))))
                    };

                    if (categoryId != 0 && isLoadSubCategory)
                    {
                        nativeSqlQuery = nativeSqlQuery.Replace("@categoryIds", "(" + catIds + ")");
                        nativateCountSqlQuery = nativateCountSqlQuery.Replace("@categoryIds", "(" + catIds + ")");
                    }

                    var products = dbContext.Products.SqlQuery(nativeSqlQuery, parameters.ToArray()).ToList();

                    totalProducts = dbContext.Database.SqlQuery<int>(nativateCountSqlQuery, parameters1.ToArray()).First();
                    foreach (var product in products)
                    {
                        product.Category = product.Category;
                        product.Images = product.Images;
                        product.SupplierProducts = product.SupplierProducts;
                        //#region Update Gian Hang
                        //var standOther = new StandOther
                        //{
                        //    StandId = 3,
                        //    ProductId = product.Id
                        //};
                        //var standTemp = dbContext.StandOthers.Where(x => x.StandId == 3 && x.ProductId == product.Id).FirstOrDefault();
                        //if (standTemp == null)
                        //{
                        //    dbContext.StandOthers.Add(standOther);
                        //    dbContext.SaveChanges();
                        //}
                        //#endregion
                    }
                    return products;
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindByCategoryId", exception);
            }

            totalProducts = 0;

            return Enumerable.Empty<Product>().ToList();
        }

        public static Product FindById(long productId)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var dealToDayCache = dbContext.DealToDayCaches.FirstOrDefault(m => m.ProductId == productId && m.Product.Status == StatusEnum.Active);

                    if (dealToDayCache != null)
                    {
                        DealToDayBll.AddByDealId(dealToDayCache.DealId);
                    }
                    
                    var product = dbContext.Set<Product>().SqlQuery("FindProductById @id", new SqlParameter("@id", productId)).AsNoTracking().Select(p => new Product
                    {
                        Id = p.Id,
                        Category = p.Category,
                        CategoryId = p.CategoryId,
                        Name = p.Name,
                        Images = p.Images.ToList(),
                        Status = p.Status,
                        Quantity = p.Quantity,
                        Price = p.Price,
                        OrderDetails = p.OrderDetails.ToList(),
                        IsFeatured = p.IsFeatured,
                        Description = p.Description,
                        ShortDescription = p.ShortDescription,
                        CreatedDate = p.CreatedDate,
                        Reviews = p.Reviews.ToList(),
                        Discounts = p.Discounts.ToList(),
                        HomePages = p.HomePages.ToList()
                    }).FirstOrDefault();

                    return product;
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindById", exception);
            }

            return null;
        }

        public static bool CheckEditableAccount(long accountId, long productId, long roleId)
        {
            bool result = false;
            if (roleId == RoleEnum.Admin)
            {
                result = true;
            }
            else
            {
                try
                {
                    using (var dbContext = new Entities())
                    {
                        //var supplierOfAccount = dbContext.SupplierAccounts.Where(m => m.AccountId == accountId).ToList();
                        //int exist = 0;
                        //for (int i = 0; i < supplierOfAccount.Count; i++)
                        //{
                        //    var supplierToCheck = supplierOfAccount[i];
                        //    var check = dbContext.SupplierProducts.Count(m => m.ProductId == productId && m.SupplierId == supplierToCheck.SupplierId);
                        //    if (check > 0)
                        //    {
                        //        exist = 1;
                        //        break;
                        //    }
                        //}
                        result = dbContext.SupplierProducts.Any(row => row.SupplierAccount.AccountId == accountId);
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("CheckEditableAccount", exception);
                }
            }
            return result;
        }

        public static bool Delete(long productId, long? userRole = RoleEnum.Admin)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var product = dbContext.Products.FirstOrDefault(m => m.Id == productId && (m.Status == StatusEnum.Active || m.Status == StatusEnum.Pending));
                    if (userRole == RoleEnum.SupplierManager || userRole == RoleEnum.SupplierEmployee)
                    {
                        if (product != null)
                        {
                            var productModel = new ProductModel
                            {
                                Id = product.Id,
                                Status = StatusEnum.InActive,
                                Name = product.Name,
                                CategoryId = product.CategoryId,
                                Quantity = product.Quantity,
                                Price = product.Price,
                                Description = product.Description,
                                ShortDescription = product.ShortDescription
                            };
                            var supplierProductDelete = dbContext.SupplierProducts.First(m => m.ProductId == productId);
                            supplierProductDelete.ExtraInfo = new JavaScriptSerializer().Serialize(productModel);
                            dbContext.SaveChanges();
                            return true;
                        }
                    }
                    if (product != null)
                    {
                        var supplierProduct = dbContext.SupplierProducts.FirstOrDefault(m => m.ProductId == product.Id && m.ExtraInfo.Length > 0);
                        if (supplierProduct != null)
                        {
                            var productModel = new JavaScriptSerializer().Deserialize<ProductModel>(supplierProduct.ExtraInfo);
                            product.Status = productModel.Status;
                            supplierProduct.ExtraInfo = string.Empty;
                        }
                        else
                        {
                            product.Status = StatusEnum.InActive;
                        }
                        dbContext.SaveChanges();
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("Delete", exception);
            }
            return false;
        }

        public static ProductModel GetNewUpdateProduct(long productId, long supplierId)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var productSupplier =
                        dbContext.SupplierProducts.First(m => m.ProductId == productId && m.SupplierId == supplierId);

                    return JsonConvert.DeserializeObject<ProductModel>(productSupplier.ExtraInfo);
                }
            }
            catch (Exception exception)
            {
                Log.Error("GetNewUpdateProduct", exception);
            }

            return null;
        }

        public static bool Approve(long productId)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var product = dbContext.Products.FirstOrDefault(m => m.Id == productId);
                    if (product != null && (product.Status == StatusEnum.Pending))
                    {
                        product.Status = StatusEnum.Active;
                        dbContext.SaveChanges();
                    }
                    else if (product != null && (product.Status == StatusEnum.Active))
                    {
                        product.Status = StatusEnum.Pending;
                        dbContext.SaveChanges();
                    }




                    return true;
                }
            }
            catch (Exception exception)
            {
                Log.Error("Approve", exception);
            }

            return false;
        }

        public static List<Product> FindAllByIds(List<long> ids)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    return
                        dbContext.Products.Where(m => ids.Contains(m.Id) && m.Category.Status == StatusEnum.Active)
                            .Include(m => m.Category)
                            .Include(m => m.Category.Category1)
                            .Include(m => m.Category.Category2)
                            .Include(m => m.Images)
                            .ToList();
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindAllByIds", exception);
            }

            return Enumerable.Empty<Product>().ToList();
        }

        public static List<Product> FindAllProductByIdIn(List<long> productIds)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    return
                        dbContext.Products.Where(m => productIds.Contains(m.Id) && m.Status == StatusEnum.Active && m.Category.Status == StatusEnum.Active)
                            .ToList();
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindAllProductByIdIn()", exception);
            }

            return Enumerable.Empty<Product>().ToList();
        }

        public static ResultModel UpdateQuantity(UserModel currentUser, long productId, int quantity)
        {
            var resultModel = new ResultModel();
            try
            {
                using (var dbContext = new Entities())
                {
                    if (currentUser == null)
                    {
                        resultModel.setCode(Result.AUTH);
                        return resultModel;
                    }

                    var acc = dbContext.Accounts.Where(m => m.Email.ToLower().Trim().Equals(currentUser.Email.ToLower().Trim())
                                                            && m.RoleId == RoleEnum.Admin);
                    if (acc != null)
                    {
                        if (quantity < 0 || quantity > 9999999)
                        {
                            resultModel.setCode(Result.INVALID_DATA);
                            return resultModel;
                        }

                        var product = dbContext.Products.Where(m => m.Id.Equals(productId)).FirstOrDefault();
                        if (product != null)
                        {
                            product.Quantity = quantity;
                            resultModel.Data = product;
                            resultModel.setCode(Result.SUCCESS);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            resultModel.setCode(Result.INVALID_DATA);
                        }
                    }
                    else
                    {
                        resultModel.setCode(Result.AUTH);
                    }

                }
            }
            catch (Exception exception)
            {
                Log.Error("FindAllByIds", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            return resultModel;

        }

        public static List<Product> FindAllProductHavingReview()
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    return dbContext.Products.Where(
                        m =>
                            m.Status == StatusEnum.Active &&
                            m.Reviews.Count(r => r.Status == StatusEnum.ReviewEnum.Status.New) > 0 &&
                            m.Category.Status == StatusEnum.Active)
                        .Include(m => m.Reviews)
                        .Include("Reviews.Account")
                        .Include(m => m.Category)
                        .Include(m => m.Images)
                        .ToList();
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindAllProductHavingReview()", exception);
            }

            return Enumerable.Empty<Product>().ToList();
        }

        public static List<Product> FindAllByProductName(string query)
        {
            if (query != null && !query.Trim().Equals(string.Empty))
            {
                try
                {
                    using (var dbContext = new Entities())
                    {
                        var nativeSqlQuery =
                            string.Format("SELECT * FROM Product WHERE Name LIKE @query AND [Status] = {0}",
                                StatusEnum.Active);

                        var products = dbContext.Products.SqlQuery(nativeSqlQuery,
                            new SqlParameter("@query", "%" + StringUtil.RemoveVietnameseTone(
                                        StringUtil.RemoveSign4VietnameseString(query.ToLower().Trim())) + "%")).ToList();

                        foreach (var product in products)
                        {
                            product.Category = product.Category;
                        }

                        return products;
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("FindAllByProductName()", exception);
                }
            }
            return Enumerable.Empty<Product>().ToList();
        }

        public static bool Update(ProductModel model)
        {
            if (model == null || model.Id == -1)
            {
                return false;
            }

            //Check role
            var User = UserBll.GetUser();

            if (User.RoleId == RoleEnum.Admin || User.RoleId == RoleEnum.Manager)
            {
                try
                {
                    using (var dbContext = new Entities())
                    {
                        using (var scope = new TransactionScope())
                        {
                            var count = 1;
                            var createdDate = DateTime.Now;

                            #region [Validation]
                            if (model.Name.Trim().Equals(string.Empty))
                            {
                                Log.Info("Product name cannot be empty.");
                                return false;
                            }

                            if (IsNameDuplicated(model.Name, model.Id))
                            {
                                Log.Info(string.Format("Product name {0} is duplicated.", model.Name));
                                return false;
                            }

                            if (model.Quantity < 0 || model.Price < 0)
                            {
                                Log.Info(model.Quantity < 0 ? "Quantity cannot be less than 0." : "Price cannot be less than 0.");
                                return false;
                            }

                            #endregion
                            var product = dbContext.Products.First(m => m.Id == model.Id && m.Status == StatusEnum.Active);

                            product.CategoryId = model.CategoryId;
                            product.Description = model.Description;
                            product.Name = model.Name;
                            product.Price = model.Price;
                            product.IsFeatured = model.IsFeatured;
                            product.Quantity = model.Quantity;
                            product.Status = StatusEnum.Active;
                            product.ShortDescription = model.ShortDescription;

                            dbContext.SaveChanges();

                            if (model.CoverId == -1)
                            {
                                var oldCover =
                                    product.Images.FirstOrDefault(
                                        m => m.Type == ImageTypeEnum.Cover && m.Status == StatusEnum.Active);

                                if (oldCover != null)
                                {
                                    var cover =
                                        dbContext.Images.First(m => m.Id == oldCover.Id && m.Status == StatusEnum.Active);
                                    cover.Status = StatusEnum.InActive;
                                    dbContext.SaveChanges();
                                }

                                if (model.Cover == null)
                                {
                                    Log.Info("Cover cannot be null.");
                                    return false;
                                }

                                var coverImg = ImageBll.Save(model.Cover, product.Id, string.Format("_{0}_{1}", count++, product.Id));
                                if (coverImg == null)
                                {
                                    Log.Error("Save image fail.");
                                    return false;
                                }

                                coverImg.Type = ImageTypeEnum.Cover;
                                coverImg.CreatedDate = createdDate;
                                dbContext.Images.Add(coverImg);
                            }


                            var oldGallery =
                                product.Images.Where(m => m.Type == ImageTypeEnum.Gallery && m.Status == StatusEnum.Active).ToList().Select(m => m.Id).ToList();

                            var deletedGallery = oldGallery.Where(galleryId => !model.GalleryIds.Contains(galleryId)).ToList();

                            var gallerys = dbContext.Images.Where(m => deletedGallery.Contains(m.Id)).ToList();
                            foreach (var gallery in gallerys)
                            {
                                gallery.Status = StatusEnum.InActive;
                            }

                            dbContext.SaveChanges();

                            foreach (var file in model.Gallery)
                            {
                                if (file == null)
                                {
                                    continue;
                                }

                                var galleryImg = ImageBll.Save(file, product.Id, string.Format("_{0}_{1}", count++, product.Id));

                                if (galleryImg == null)
                                {
                                    Log.Error("Save image fail.");
                                    return false;
                                }

                                galleryImg.Type = ImageTypeEnum.Gallery;
                                galleryImg.CreatedDate = createdDate;
                                dbContext.Images.Add(galleryImg);
                            }

                            dbContext.SaveChanges();

                            scope.Complete();

                            return true;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("Update", exception);
                }

            }

            else
            {
                var json = new JavaScriptSerializer().Serialize(model);
                try
                {
                    using (var dbContext = new Entities())
                    {
                        using (var scope = new TransactionScope())
                        {
                            var productSupplierToUpdate = dbContext.SupplierProducts.FirstOrDefault(m => m.ProductId == model.Id);
                            productSupplierToUpdate.ExtraInfo = json;
                            dbContext.SaveChanges();
                            scope.Complete();

                            return true;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("Update", exception);
                }
            }

            return false;
        }

        public static List<Product> FindAllRelatedProduct(long productId)
        {
            const int max = 3;
            try
            {
                using (var dbContext = new Entities())
                {
                    var product = dbContext.Products.FirstOrDefault(m => m.Id == productId);

                    if (product != null)
                    {
                        var rootCategory =
                            dbContext.Database.SqlQuery<FindRootCategory_Result>("FindRootCategory @categoryId ",
                                new SqlParameter("@categoryId", product.CategoryId)).FirstOrDefault();

                        var catIds = new List<long>() { product.CategoryId, rootCategory.Id.HasValue ? rootCategory.Id.Value : 0 };

                        return
                            dbContext.Products.Where(
                                m =>
                                    m.Status == StatusEnum.Active && catIds.Contains(m.CategoryId) &&
                                    m.Category.Status == StatusEnum.Active)
                                .Include(m => m.Category)
                                .Include(m => m.Images)
                                .Take(max)
                                .ToList();
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindAllRelatedProduct", exception);
            }

            return Enumerable.Empty<Product>().ToList();
        }

        public static List<Product> FindAllBySupplierId(long supplierid)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    return
                        dbContext.SupplierProducts.Where(
                            m => m.SupplierId == supplierid && m.Status == StatusEnum.Active)
                            .Select(m => m.Product)
                            .Include(m => m.Images)
                            .Include(m => m.Category)
                            .ToList();
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindAllByAccountId", exception);
            }
            return Enumerable.Empty<Product>().ToList();
        }

        public static FindAllProductModel FindAllProduct(long categoryId, string sort = "default", int order = 1,
            int limit = 6, int page = 1, string query = "", int status = StatusEnum.Active, long? supplierId = null)
        {
            query = query.Trim();

            var categories = CategoryBll.FindAllCategories();
            var category = CategoryBll.FindById(categoryId);
            int totalProduct;
            var products = ProductBll.FindByCategoryId(categoryId, sort, order, limit, page, query, out totalProduct, true, status, supplierId);

            if (categoryId != 0 && category == null)
            {
                throw new HttpException(500, "Category Not Found.");
            }

            if (categoryId == 0)
            {
                category = new Category
                {
                    Id = 0,
                    Name = "Tất cả Danh Mục"
                };
            }

            return new FindAllProductModel
            {
                Category = new CategoryManageList(category),
                Categories = categories.Select(cat => new CategoryManageList(cat)).ToList(),
                Products = products.Select(p => new ProductThumbnailModel(p)).ToList(),
                DisplayStyle = "grid",
                Sort = sort,
                Order = order,
                TotalProduct = totalProduct,
                Page = page,
                Limit = limit,
                Query = query
            };
        }

        public static FindAllProductModel FindAllDealToDayProduct(string sort = "default", int order = 1,
            int limit = 6, int page = 1, string query = "", int status = StatusEnum.Active, long? supplierId = null)
        {
            query = query.Trim();

            var categories = CategoryBll.FindAllCategories();
            var category = CategoryBll.FindDealToDayCategory();
            int totalProduct;
            var products = ProductBll.FindByCategoryId(category.Id, sort, order, limit, page, query, out totalProduct, true, status, supplierId);

            return new FindAllProductModel
            {
                Category = new CategoryManageList(category),
                Categories = categories.Select(cat => new CategoryManageList(cat)).ToList(),
                Products = products.Select(p => new ProductThumbnailModel(p)).ToList(),
                DisplayStyle = "grid",
                Sort = sort,
                Order = order,
                TotalProduct = totalProduct,
                Page = page,
                Limit = limit,
                Query = query
            };
        }

        public static FindAllProductModel FindAllHomeProduct(long categoryId = 0, string sort = "default", int order = 1,
            int limit = 6, int page = 1, long typeHomePage = 1)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var categoryIds = new List<long>();
                    var categories = new List<Category>();

                    var category = new Category
                    {
                        Id = 0,
                        Name = "Sản Phẩm Mới Nhất"
                    };

                    switch (typeHomePage)
                    {
                        case TypeHomePageEnum.Latest:
                            category.Name = "Sản Phẩm Mới Nhất";
                            break;
                        case TypeHomePageEnum.Bestsellers:
                            category.Name = "Sản Phẩm Bán Chạy";
                            break;
                    }

                    if (categoryId != 0)
                    {
                        categoryIds.Add(categoryId);

                        category = dbContext.Categories.FirstOrDefault(m => m.Id == categoryId);

                        if (category == null)
                        {
                            Log.Info("Category Id " + categoryId + " not found");
                            return null;
                        }
                    }
                    else
                    {
                        categories =
                            dbContext.Categories.Where(
                                m =>
                                    m.ParentCategoryId.Equals(null) &&
                                    m.Status == StatusEnum.Active).ToList();

                        categoryIds = categories.Select(m => m.Id).ToList();
                    }

                    var homepageQuery = dbContext.HomePages.Where(
                        m =>
                            m.Status == StatusEnum.Active && m.TypeHomePageId == typeHomePage &&
                            categoryIds.Contains(m.Product.CategoryId)).Select(m => m.Product);

                    var totalProduct = homepageQuery.Count();

                    switch (sort)
                    {
                        case SortLatest:
                            homepageQuery = order == Asc
                                ? homepageQuery.OrderBy(m => m.CreatedDate)
                                : homepageQuery.OrderByDescending(m => m.CreatedDate);
                            break;
                        case SortPrice:
                            homepageQuery = order == Asc
                                ? homepageQuery.OrderBy(m => m.Price)
                                : homepageQuery.OrderByDescending(m => m.Price);
                            break;
                        default:
                            homepageQuery = order == Asc
                                ? homepageQuery.OrderBy(m => m.Name)
                                : homepageQuery.OrderByDescending(m => m.Name);
                            break;
                    }


                    homepageQuery = homepageQuery.Take(limit).Skip((page - 1) * limit).Include(m => m.Category)
                        .Include(m => m.Category.Category1)
                        .Include(m => m.Category.Category2)
                        .Include(m => m.Images);

                    var products = homepageQuery.ToList();

                    return new FindAllProductModel
                    {
                        Category = new CategoryManageList(category),
                        Categories = categories.Select(cat => new CategoryManageList(cat)).ToList(),
                        Products = products.Select(p => new ProductThumbnailModel(p)).ToList(),
                        DisplayStyle = "grid",
                        Sort = sort,
                        Order = order,
                        TotalProduct = totalProduct,
                        Page = page,
                        Limit = limit,
                        Query = string.Empty
                    };
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindAllHomeProduct()", exception);
            }
            return null;
        }
        public static bool IsDealToDayProduct(long productId)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var productDealToDay = dbContext.SupplierProducts.Where(m => m.ProductId.Equals(productId) && m.Supplier.Type.Equals(SupplierTypeEnum.DealToDay)).FirstOrDefault();
                    if (productDealToDay != null)
                    {
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {

                Log.Error("IsDealToDayProduct", exception);
            }
            return false;
        }
        public static FindAllProductModel FindAllStandProduct(long standId = 0, string sort = "default", int order = 1,
            int limit = 6, int page = 1)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var categoryIds = new List<long>();
                    var lstProductIds = new List<long>();
                    var categories = new List<Category>();
                    StandModel standInfor = new StandModel();
                    var category = new Category
                    {
                        Id = 0,
                        Name = "Sản Phẩm Mới Nhất"
                    };
                    var stand = dbContext.Stands.FirstOrDefault(x => x.StandId == standId);
                    if (stand != null)
                    {
                        standInfor = new StandModel
                        {
                            Id = stand.Id,
                            StandId = stand.StandId,
                            StandName = stand.StandName,
                            Address = stand.Address,
                            Phone = stand.Phone,
                            CreatedDate = Convert.ToDateTime(stand.CreatedDate),
                            CreatedBy = Convert.ToInt32(stand.CreatedBy),
                            UpdateDate = Convert.ToDateTime(stand.UpdateDate),
                            Status = Convert.ToInt32(stand.Status),
                            Banner=stand.Banner,
                            Description=stand.Description,
                            PayMethod=stand.PayMethod
                        };
                    }


                    var listProductIds = (from n in dbContext.StandOthers
                                          where n.StandId== standId
                                          select n.ProductId).ToList();
                    var homepageQuery = dbContext.Products.Where(m => listProductIds.Contains(m.Id));
                    
                    var totalProduct = homepageQuery.Count();
                    switch (sort)
                    {
                        case SortLatest:
                            homepageQuery = order == Asc
                                ? homepageQuery.OrderBy(m => m.CreatedDate)
                                : homepageQuery.OrderByDescending(m => m.CreatedDate);
                            break;
                        case SortPrice:
                            homepageQuery = order == Asc
                                ? homepageQuery.OrderBy(m => m.Price)
                                : homepageQuery.OrderByDescending(m => m.Price);
                            break;
                        default:
                            homepageQuery = order == Asc
                                ? homepageQuery.OrderBy(m => m.Name)
                                : homepageQuery.OrderByDescending(m => m.Name);
                            break;
                    }


                    //homepageQuery = homepageQuery.Take(limit).Skip((page - 1) * limit).Include(m => m.Category)
                    //    .Include(m => m.Category.Category1)
                    //    .Include(m => m.Category.Category2)
                    //    .Include(m => m.Images);
                    homepageQuery = homepageQuery.Skip((page - 1) * limit).Take(limit);

                    var products = homepageQuery.ToList();

                    return new FindAllProductModel
                    {
                        StandInfor = standInfor,
                        Category = new CategoryManageList(category),
                        Categories = categories.Select(cat => new CategoryManageList(cat)).ToList(),
                        Products = products.Select(p => new ProductThumbnailModel(p)).ToList(),
                        DisplayStyle = "grid",
                        Sort = sort,
                        Order = order,
                        TotalProduct = totalProduct,
                        Page = page,
                        Limit = limit,
                        Query = string.Empty
                    };
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindAllStandProduct()", exception);
            }
            return null;
        }
        public static StandModel GetStandByProductId(long productId)
        {
            StandModel standModel = new StandModel();
            try
            {
                
                using (var dbContext = new Entities())
                {
                    var stand = (from s in dbContext.Stands
                                join so in dbContext.StandOthers on s.StandId equals so.StandId
                                where so.ProductId == productId
                                select s).FirstOrDefault();
                    if (stand != null)
                    {
                        standModel = new StandModel
                        {
                            Id = stand.Id,
                            StandId = stand.StandId,
                            StandName = stand.StandName,
                            Address = stand.Address,
                            Phone = stand.Phone,
                            CreatedDate = Convert.ToDateTime(stand.CreatedDate),
                            CreatedBy = Convert.ToInt32(stand.CreatedBy),
                            UpdateDate = Convert.ToDateTime(stand.UpdateDate),
                            Status = Convert.ToInt32(stand.Status),
                            Banner = stand.Banner,
                            Description = stand.Description,
                            PayMethod = stand.PayMethod
                        };
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("GetStandByProductId", exception);
            }

            return standModel;
        }
    }
}