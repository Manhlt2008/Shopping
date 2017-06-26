using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Transactions;
using System.Web;
using log4net;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.User;


namespace WebApplication.Lib.Bll
{
    public static class SupplierBll
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public enum SystemSupplierEnum
        {
            DealToDay
        }

        public static Supplier FindOneById(long id)
        {
            Supplier supplier = new Supplier();
            try
            {
                using (var dbContext = new Entities())
                {
                    supplier = dbContext.Suppliers.Where(m => m.Id == id).Include("SupplierCategories.Category").FirstOrDefault();
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindOneById()", exception);
            }
            return supplier;
        }

        public static List<Supplier> ListSupplierByAccount(long accountId)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var listSupplierIds = dbContext.SupplierAccounts.Where(m => m.AccountId == accountId).ToList();
                    var suppliers = new List<Supplier>();
                    foreach (var supplierAccount in listSupplierIds)
                    {
                        var supplier = dbContext.Suppliers.First(m => m.Id == supplierAccount.SupplierId);
                        suppliers.Add(supplier);
                    }
                    return suppliers;
                }
            }
            catch (Exception exception)
            {
                Log.Error("ListSupplierByAccount()", exception);
            }
            return null;
        }

        public static Supplier FindOneByProductId(long productId)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var supplier = dbContext.SupplierProducts.First(m=>m.ProductId==productId).Supplier;
                    return supplier;
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindOneByProductId()", exception);
            }
            return null;
        }

        public static List<Supplier> FindAllBySupplierName(string query)
        {
            if (query != null)
            {
                try
                {
                    using (var dbContext = new Entities())
                    {
                        var nativeSqlQuery =
                            string.Format("SELECT * FROM Supplier WHERE Name LIKE @query");

                        var suppliers = dbContext.Suppliers.SqlQuery(nativeSqlQuery,
                            new SqlParameter("@query", "%" + StringUtil.RemoveVietnameseTone(
                                        StringUtil.RemoveSign4VietnameseString(query.ToLower().Trim())) + "%")).ToList();

                        foreach (var supplier in suppliers)
                        {
                            supplier.SupplierAccounts = supplier.SupplierAccounts;
                            supplier.SupplierCategories = supplier.SupplierCategories;
                            supplier.SupplierProducts = supplier.SupplierProducts;
                        }

                        return suppliers;
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("FindAllBySupplierName()", exception);
                }
            }
            return Enumerable.Empty<Supplier>().ToList();
        }

        public static bool CreatProductBySupplier(SupplierProduct model)
        {
            try
            {
                using (var dbContext = new Entities())
                {

                    using (var scope = new TransactionScope())
                    {
                        var supplierproduct = new SupplierProduct
                        {
                            SupplierId = model.SupplierId,
                            CreatedSupplierAccountId = model.CreatedSupplierAccountId,
                            ProductId = model.ProductId,
                            SupplierCategoryId = model.SupplierCategoryId,
                            Status = StatusEnum.InActive
                        };
                        dbContext.SupplierProducts.Add(supplierproduct);
                        dbContext.SaveChanges();
                        scope.Complete();
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {

                Log.Error("CreatProductBySupplier", exception);
            }
            return false;
        }

        public static bool Create(Models.Supplier.Supplier model, int type = SupplierTypeEnum.Normal)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        var supplier = new Supplier
                        {
                            Name = model.Name,
                            Address = model.Address,
                            Email = model.Email,
                            Phone = model.Phone,
                            Website = model.Website,
                            Facebook = model.Facebook,
                            Status = StatusEnum.Active,
                            Type = type
                        };

                        if (model.Id != 0)
                        {
                            // update
                            supplier = dbContext.Suppliers.FirstOrDefault(m => m.Id == model.Id);
                            if (supplier == null)
                            {
                                return false;
                            }
                            // Update data
                            supplier.Name = model.Name;
                            supplier.Address = model.Address;
                            supplier.Email = model.Email;
                            supplier.Phone = model.Phone;
                            supplier.Website = model.Website;

                            if (model.AccountIds.Count > 0)
                            {
                                //Update Accounts Data
                                List<SupplierAccount> oldAccount = dbContext.SupplierAccounts.Where(m => m.SupplierId == model.Id).ToList();

                                //Check for existed accounts
                                foreach (var item in oldAccount)
                                {
                                    var exist = 0;
                                    for (int i = 0; i < model.AccountIds.Count; i++)
                                    {
                                        if (item.AccountId == model.AccountIds[i])
                                        {
                                            var itemToUpdate = dbContext.SupplierAccounts.Find(item.Id);
                                            itemToUpdate.Status = StatusEnum.Active;
                                            exist = 1;
                                            model.AccountIds.RemoveAt(i);
                                            dbContext.SaveChanges();
                                            break;
                                        }
                                    }
                                    if (exist == 0)
                                    {
                                        var itemToUpdate = dbContext.SupplierAccounts.Find(item.Id);
                                        itemToUpdate.Status = StatusEnum.InActive;
                                        dbContext.SaveChanges();
                                    }
                                }

                                //For new Account, add to the list of this supplier
                                if (model.AccountIds.Count > 0)
                                {
                                    foreach (var item in model.AccountIds)
                                    {
                                        if (item != 0)
                                        {
                                            var sa = new SupplierAccount
                                            {
                                                SupplierId = model.Id,
                                                AccountId = item,
                                                Status = StatusEnum.Active
                                            };

                                            dbContext.SupplierAccounts.Add(sa);
                                            dbContext.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // create
                            dbContext.Suppliers.Add(supplier);
                        }

                        dbContext.SaveChanges();

                        // TODO: Viet Tuong Tu
                        if (model.Id == 0 && model.UserModel != null && (model.AccountIds == null || model.AccountIds.Count == 0))
                        {
                            var resultmodel = UserBll.CreateUser(model.UserModel, RoleEnum.SupplierManager);
                            var account = (Account)resultmodel.Data;

                            var sa = new SupplierAccount
                            {
                                SupplierId = supplier.Id,
                                AccountId = account.Id,
                                Status = StatusEnum.Active
                            };

                            dbContext.SupplierAccounts.Add(sa);
                            dbContext.SaveChanges();
                        }

                        //Add existed Accounts when create supplier
                        if (model.Id == 0 && model.UserModel == null && model.AccountIds.Count > 0)
                        {
                            foreach (var item in model.AccountIds)
                            {
                                if (item != 0)
                                {
                                    var sa = new SupplierAccount
                                    {
                                        SupplierId = supplier.Id,
                                        AccountId = item,
                                        Status = StatusEnum.Active
                                    };

                                    dbContext.SupplierAccounts.Add(sa);
                                    dbContext.SaveChanges();
                                }
                            }
                        }

                        var existedCategories = dbContext.SupplierCategories.Where(m => m.SupplierId == supplier.Id).ToList();

                        // Set to inactivate
                        foreach (var supplierCategory in existedCategories)
                        {
                            supplierCategory.Status = StatusEnum.InActive;
                        }

                        foreach (var categoryId in model.CategoryIds)
                        {
                            var existedCat = existedCategories.FirstOrDefault(m => m.CategoryId == categoryId);

                            if (existedCat == null)
                            {
                                // add more
                                dbContext.SupplierCategories.Add(new SupplierCategory
                                {
                                    SupplierId = supplier.Id,
                                    CategoryId = categoryId,
                                    Status = StatusEnum.Active
                                });
                            }
                            else
                            {
                                // update
                                existedCat.Status = StatusEnum.Active;
                            }
                        }

                        //                        var cats = new List<SupplierCategory>();
                        //                        foreach (var categoryId in model.CategoryIds)
                        //                        {
                        //                            cats.Add(new SupplierCategory
                        //                            {
                        //                                SupplierId = supplier.Id,
                        //                                CategoryId = categoryId,
                        //                                Status = StatusEnum.Active
                        //                            });
                        //                        }
                        //                        dbContext.SupplierCategories.AddRange(cats);
                        dbContext.SaveChanges();

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

        public static Supplier GetSupplierByUserSession()
        {
            var supplier = (Supplier)HttpContext.Current.Session[Constant.Supplier];

            if (supplier == null)
            {
                try
                {
                    using (var dbContext = new Entities())
                    {
                        var user = UserBll.GetUser();
                        if (user == null)
                        {
                            Log.Info("GetSupplierByUserId(). Cannot get supplier due to account session not found.");
                        }
                        else
                        {
                            supplier = dbContext.SupplierAccounts.First(m => m.AccountId == user.Id).Supplier;
                            var sa = supplier.SupplierAccounts.First(m => m.AccountId == user.Id);

                            supplier.SupplierAccounts.Clear();
                            supplier.SupplierAccounts.Add(sa);

                            HttpContext.Current.Session[Constant.Supplier] = supplier;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("GetSupplierByUserId", exception);
                }
            }

            return supplier;

        }

        public static ResultModel UpdateStatusSupplier(UserModel currentUser, string email, byte status)
        {
            var resultModel = new ResultModel();
            var userModel = new UserModel();
            try
            {
                using (var dbContext = new Entities())
                {

                    #region [Validation]
                    if (currentUser == null)
                    {
                        resultModel.setCode(Result.AUTH);
                        return resultModel;
                    }
                    if (status != StatusEnum.InActive && status != StatusEnum.Active)
                    {
                        resultModel.setCode(Result.INVALID_DATA);
                        return resultModel;
                    }
                    #endregion

                    #region [ListAllUser]
                    var supplier = dbContext.Suppliers.FirstOrDefault(m => m.Email.Trim().Equals(email.Trim()));
                    if (supplier != null)
                    {

                        supplier.Status = status;
                        dbContext.SaveChanges();
                        resultModel.setCode(Result.SUCCESS);
                        resultModel.Data = supplier;
                    }
                    else
                    {
                        resultModel.setCode(Result.FAILED);
                    }


                    #endregion
                }
            }
            catch (Exception exception)
            {
                Log.Error("UpdateStatusSupplier", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            return resultModel;
        }

        public static long FindOneDefaultSupplierIdByAccountId(long accountId)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    return
                        dbContext.Accounts.First(m => m.Id == accountId)
                            .SupplierAccounts.First(m => m.Status == StatusEnum.Active)
                            .SupplierId;
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindOneDefaultSupplierIdByAccountId", exception);
            }
            return long.MinValue;
        }
    }
}