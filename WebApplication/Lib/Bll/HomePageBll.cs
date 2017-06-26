using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.HomPage;
using WebApplication.Models.User;

namespace WebApplication.Lib.Bll
{
    public class HomePageBll
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static ResultModel SaveListHomePages(List<HomePageModel> homePages)
        {
            var resultModel = new ResultModel();
            if (homePages.Count > 0)
            {
                var isValid = true;
                var itemFail = new List<HomePageModel>();
                foreach (var item in homePages)
                {
                    item.StartDate = DateTime.Now;
                    item.EndDate = DateTime.Now;

                    resultModel = SaveHomePage(item);
                    if (resultModel.Code != Result.SUCCESS.Code)
                    {
                        isValid = false;
                        resultModel.setCode(Result.FAILED);
                        itemFail.Add(item);
                    }
                }

                if (isValid)
                    resultModel.setCode(Result.SUCCESS);
                else
                {
                    resultModel.setCode(Result.FAILED);
                    resultModel.Data = itemFail;
                }
            }

            return resultModel;
        }
        public static ResultModel SaveHomePage(HomePageModel homePageModel)
        {
            var resultModel = new ResultModel();
            var homePage = new HomePage();
            if (homePageModel.Id != 0)
            {
                homePage = new HomePage
                {
                    Id = homePageModel.Id,
                    ProductId = homePageModel.ProductId,
                    StartDate = homePageModel.StartDate,
                    Endate = homePageModel.EndDate,
                    IOrder = homePageModel.IOrder,
                    TypeHomePageId = homePageModel.TypeHomePageId,
                    Status = homePageModel.Status
                };
            }
            else
            {
                homePage = new HomePage
                {
                    ProductId = homePageModel.ProductId,
                    StartDate = homePageModel.StartDate,
                    Endate = homePageModel.EndDate,
                    TypeHomePageId = homePageModel.TypeHomePageId,
                    Status = homePageModel.Status
                };
            }

            try
            {
                using (var dbContext = new Entities())
                {
                    #region [Validate]
                    // Kiểm tra dữ liệu 
                    if (homePageModel.StartDate == null || homePageModel.EndDate == null)
                    {
                        resultModel.setCode(Result.INVALID_DATA);
                        resultModel.Message = resultModel.Message + " [Null value]";
                        return resultModel;
                    }

                    //StartDate - EndDate
                    //IOrder
                    if (homePageModel.IOrder < 0)
                    {
                        resultModel.setCode(Result.INVALID_DATA);
                        resultModel.Message = resultModel.Message + " [IOrder]";
                        return resultModel;
                    }
                    //Type 
                    if (!homePageModel.TypeHomePageId.Equals(TypeHomePageEnum.Featured) &&
                        !homePageModel.TypeHomePageId.Equals(TypeHomePageEnum.Latest) &&
                        !homePageModel.TypeHomePageId.Equals(TypeHomePageEnum.Specials) &&
                        !homePageModel.TypeHomePageId.Equals(TypeHomePageEnum.Bestsellers))
                    {
                        resultModel.setCode(Result.INVALID_DATA);
                        resultModel.Message = resultModel.Message + " [Type]";
                        return resultModel;
                    }
                    //Status
                    if (!homePageModel.Status.Equals(StatusEnum.Active) && !homePageModel.Status.Equals(StatusEnum.InActive))
                    {
                        resultModel.setCode(Result.INVALID_DATA);
                        resultModel.Message = resultModel.Message + " [Status]";
                        return resultModel;
                    }

                    #endregion

                    #region [Save HomePage]
                    // Id
                    var home = dbContext.HomePages.Where(m => m.Id.Equals(homePageModel.Id) ||
                        (m.ProductId.Equals(homePage.ProductId) && m.TypeHomePageId.Equals(homePage.TypeHomePageId))).FirstOrDefault();
                    if (home == null)
                    {
                        var listHome = dbContext.HomePages.Where(m => m.TypeHomePageId.Equals(homePage.TypeHomePageId)
                                                                && m.Status.Equals(StatusEnum.Active)).ToList<HomePage>();
                        homePage.IOrder = listHome.Count + 1;
                        dbContext.HomePages.Add(homePage);
                        if (dbContext.SaveChanges() > 0)
                        {
                            resultModel.Data = homePage;
                            resultModel.setCode(Result.SUCCESS);
                        }
                    }
                    else
                    {
                        home.StartDate = homePage.StartDate;
                        home.Endate = homePage.Endate;
                        home.Status = homePage.Status;

                        dbContext.SaveChanges();

                        resultModel.Data = homePage;
                        resultModel.setCode(Result.SUCCESS);
                    }

                    #endregion
                }
            }
            catch (Exception exception)
            {
                Log.Error("Add Or Update HomePage", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            return resultModel;
        }

        public static ResultModel IncreateIOrder(UserModel user, long homePageId)
        {
            var resultModel = new ResultModel();
            var homePage = new HomePage();
            if (homePageId != 0)
            {
                homePage = new HomePage
                {
                    Id = homePageId,
                };
            }
            else
            {
                resultModel.setCode(Result.INVALID_DATA);
                resultModel.Message = resultModel.Message + " [Null ID]";
                return resultModel;
            }

            try
            {
                using (var dbContext = new Entities())
                {
                    using (var dbContextTransaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            #region [Validate]
                            if (user == null || !user.RoleId.Equals((int)RoleEnum.Admin))
                            {
                                resultModel.setCode(Result.AUTH);
                                return resultModel;
                            }
                            #endregion

                            #region [Save HomePage]
                            // Id
                            var home = dbContext.HomePages.Where(m => m.Id.Equals(homePageId)).FirstOrDefault();
                            if (home == null)
                            {
                                resultModel.setCode(Result.FAILED);
                            }
                            else
                            {
                                var highIOrder = home.IOrder - 1;
                                if (highIOrder == 0)
                                {
                                    resultModel.setCode(Result.FAILED);
                                }
                                else
                                {
                                    var homeHigh = dbContext.HomePages.Where(m => m.IOrder.Equals(highIOrder)
                                                                    && m.TypeHomePageId.Equals(home.TypeHomePageId)
                                                                    && m.Status.Equals(StatusEnum.Active)).FirstOrDefault();
                                    var temp = home.IOrder;
                                    home.IOrder = homeHigh.IOrder;

                                    homeHigh.IOrder = temp;
                                    dbContext.SaveChanges();

                                    resultModel.Data = homePage;
                                    resultModel.setCode(Result.SUCCESS);
                                }
                            }
                            #endregion

                            dbContextTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            dbContextTransaction.Rollback();
                            resultModel.setCode(Result.SYSTEM);
                        }
                    }

                }
            }
            catch (Exception exception)
            {
                Log.Error("Add Or Update HomePage", exception);
                resultModel.setCode(Result.SYSTEM);
                throw exception;
            }

            return resultModel;
        }

        public static ResultModel DecreaseIOrder(UserModel user, long homePageId)
        {
            var resultModel = new ResultModel();
            var homePage = new HomePage();
            if (homePageId != 0)
            {
                homePage = new HomePage
                {
                    Id = homePageId,
                };
            }
            else
            {
                resultModel.setCode(Result.INVALID_DATA);
                resultModel.Message = resultModel.Message + " [Null ID]";
                return resultModel;
            }

            try
            {
                using (var dbContext = new Entities())
                {
                    using (var dbContextTransaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            #region [Validate]
                            if (user == null || !user.RoleId.Equals((int)RoleEnum.Admin))
                            {
                                resultModel.setCode(Result.AUTH);
                                return resultModel;
                            }
                            #endregion

                            #region [Save HomePage]
                            // Id
                            var home = dbContext.HomePages.Where(m => m.Id.Equals(homePageId)).FirstOrDefault();
                            if (home == null)
                            {
                                resultModel.setCode(Result.FAILED);
                            }
                            else
                            {
                                var listHome = dbContext.HomePages.Where(m => m.TypeHomePageId.Equals(home.TypeHomePageId)
                                                                       && m.Status.Equals(StatusEnum.Active)).ToList<HomePage>();
                                int lowOrder = home.IOrder + 1;
                                if (lowOrder > listHome.Count)
                                {
                                    resultModel.setCode(Result.FAILED);
                                }
                                else
                                {
                                    var homeLow = dbContext.HomePages.Where(m => m.IOrder.Equals(lowOrder)
                                            && m.TypeHomePageId.Equals(home.TypeHomePageId)
                                            && m.Status.Equals(StatusEnum.Active)).FirstOrDefault();
                                    if (homeLow != null)
                                    {
                                        var temp = home.IOrder;
                                        home.IOrder = homeLow.IOrder;

                                        homeLow.IOrder = temp;
                                        dbContext.SaveChanges();

                                        resultModel.Data = homePage;
                                        resultModel.setCode(Result.SUCCESS);
                                    }
                                    else
                                    {
                                        resultModel.setCode(Result.FAILED);
                                    }
                                }
                            }

                            #endregion

                            dbContextTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            dbContextTransaction.Rollback();
                            resultModel.setCode(Result.SYSTEM);
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("Add Or Update HomePage", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            return resultModel;
        }

        public static ResultModel getHomePageByType(string type)
        {
            var resultModel = new ResultModel();
            try
            {
                using (var dbContext = new Entities())
                {
                    #region [Validation]
                    if (type != null)
                    {
                        if (!type.Equals(TypeHomePageEnum.Featured) &&
                            !type.Equals(TypeHomePageEnum.Latest) &&
                            !type.Equals(TypeHomePageEnum.Specials) &&
                            !type.Equals(TypeHomePageEnum.Bestsellers))
                        {
                            resultModel.setCode(Result.INVALID_DATA);
                            resultModel.Message = resultModel.Message + " [Type]";
                            return resultModel;
                        }
                    }
                    #endregion
                    #region [Get HomePage by Type]
                    var homePages = dbContext.HomePages.Where(m => m.TypeHomePageId.Equals(type)).ToList<HomePage>();
                    List<HomePageModel> listHomePage = new List<HomePageModel>();
                    if (homePages != null)
                    {
                        foreach (var item in homePages)
                        {
                            HomePageModel home = new HomePageModel();

                            home.Id = item.Id;
                            home.ProductId = item.ProductId;
                            home.StartDate = (DateTime)item.StartDate;
                            home.EndDate = (DateTime)item.Endate;
                            home.IOrder = item.IOrder;
                            home.TypeHomePageId = item.TypeHomePageId;
                            home.Status = item.Status;

                            listHomePage.Add(home);
                        }

                        resultModel.setCode(Result.SUCCESS);
                    }

                    resultModel.Data = listHomePage;
                    #endregion
                }
            }
            catch (Exception exception)
            {
                Log.Error("Get HomePage by Type", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            return resultModel;
        }

        public static ResultModel getHomePageByTypeAndProductId(long typeId, long productId)
        {
            var resultModel = new ResultModel();
            try
            {
                using (var dbContext = new Entities())
                {
                    #region [Validation]
                    if (!typeId.Equals(TypeHomePageEnum.Featured) &&
                        !typeId.Equals(TypeHomePageEnum.Latest) &&
                        !typeId.Equals(TypeHomePageEnum.Specials) &&
                        !typeId.Equals(TypeHomePageEnum.Bestsellers))
                    {
                        resultModel.setCode(Result.INVALID_DATA);
                        resultModel.Message = resultModel.Message + " [Type]";
                        return resultModel;
                    }
                    #endregion
                    #region [Get HomePage by Type]
                    var homePage = dbContext.HomePages.FirstOrDefault(m => m.TypeHomePageId.Equals(typeId) && m.ProductId.Equals(productId));
                    if (homePage != null)
                    {
                        HomePageModel home = new HomePageModel();

                        home.Id = homePage.Id;
                        home.ProductId = homePage.ProductId;
                        home.StartDate = (DateTime)homePage.StartDate;
                        home.EndDate = (DateTime)homePage.Endate;
                        home.IOrder = homePage.IOrder;
                        home.TypeHomePageId = homePage.TypeHomePageId;
                        home.Status = homePage.Status;

                        resultModel.setCode(Result.SUCCESS);
                        resultModel.Data = home;
                    }
                    #endregion
                }
            }
            catch (Exception exception)
            {
                Log.Error("Get HomePage by Type And ProductId", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            return resultModel;
        }

        public static ResultModel getHomePageByProductId(UserModel currentUser, long productId)
        {
            var resultModel = new ResultModel();
            try
            {
                using (var dbContext = new Entities())
                {
                    #region [Validation]
                    if (currentUser == null || !currentUser.RoleId.Equals(RoleEnum.Admin))
                    {
                        resultModel.setCode(Result.AUTH);
                        return resultModel;
                    }
                    #endregion
                    #region [Get HomePage by Type]
                    var homePages = dbContext.HomePages.Where(m => m.ProductId.Equals(productId)).ToList<HomePage>();
                    List<HomePageModel> listHomePage = new List<HomePageModel>();
                    if (homePages != null)
                    {
                        foreach (var item in homePages)
                        {
                            HomePageModel home = new HomePageModel();

                            home.Id = item.Id;
                            home.ProductId = item.ProductId;
                            home.StartDate = (DateTime)item.StartDate;
                            home.EndDate = (DateTime)item.Endate;
                            home.IOrder = item.IOrder;
                            home.TypeHomePageId = item.TypeHomePageId;
                            home.Status = item.Status;
                            home.TypeHomePageName = item.TypeHomePage.TypeName;

                            listHomePage.Add(home);
                        }

                        var typeHomePages = dbContext.TypeHomePages.ToList<TypeHomePage>();

                        if (listHomePage.Count < 4)
                        {
                            foreach (var typeHome in typeHomePages)
                            {
                                var isExisted = false;
                                if (listHomePage.Count > 0)
                                {
                                    foreach (var listHome in listHomePage)
                                    {
                                        if (typeHome.Id.Equals(listHome.TypeHomePageId))
                                        {
                                            isExisted = true;
                                        }
                                    }
                                }

                                if (!isExisted)
                                {
                                    HomePageModel home = new HomePageModel();

                                    home.ProductId = productId;
                                    home.TypeHomePageId = typeHome.Id;
                                    home.TypeHomePageName = typeHome.TypeName;
                                    home.StartDate = DateTime.Now;
                                    home.EndDate = DateTime.Now;
                                    home.Status = 0;

                                    listHomePage.Add(home);
                                }
                            }
                        }

                        resultModel.setCode(Result.SUCCESS);
                    }

                    resultModel.Data = listHomePage;
                    #endregion
                }
            }
            catch (Exception exception)
            {
                Log.Error("Get HomePage by producID", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            return resultModel;
        }

        public static ResultModel getHomePageByStatus(int status, long typeHomePageId)
        {
            var resultModel = new ResultModel();
            try
            {
                using (var dbContext = new Entities())
                {
                    #region [Validation]
                    if (!status.Equals(StatusEnum.Active) && !status.Equals(StatusEnum.InActive))
                    {
                        resultModel.setCode(Result.INVALID_DATA);
                        resultModel.Message = resultModel.Message + " [Status]";
                        return resultModel;
                    }
                    #endregion
                    #region [Get HomePage by Status]

                    var homePages = dbContext.HomePages.Where(m => m.Status.Equals(status)
                                            && m.TypeHomePageId.Equals(typeHomePageId)).ToList<HomePage>().OrderBy(m => m.IOrder);
                    if (typeHomePageId.Equals(TypeHomePageEnum.AllTypeHomePage))
                    {
                        dbContext.HomePages.Where(m => m.Status.Equals(status)).ToList<HomePage>().OrderBy(m => m.IOrder);
                    }

                    List<HomePageModel> listHomePage = new List<HomePageModel>();
                    if (homePages != null)
                    {
                        foreach (var item in homePages)
                        {
                            HomePageModel home = new HomePageModel();

                            home.Id = item.Id;
                            home.ProductId = item.ProductId;
                            home.ProductName = item.Product.Name;
                            home.StartDate = (DateTime)item.StartDate;
                            home.EndDate = (DateTime)item.Endate;
                            home.IOrder = item.IOrder;
                            home.TypeHomePageId = item.TypeHomePageId;
                            home.TypeHomePageName = item.TypeHomePage.TypeName;
                            home.Status = item.Status;

                            listHomePage.Add(home);
                        }

                        resultModel.setCode(Result.SUCCESS);
                    }

                    resultModel.Data = listHomePage;
                    #endregion
                }
            }
            catch (Exception exception)
            {
                Log.Error("Get HomePage by Type", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            return resultModel;
        }
    }
}