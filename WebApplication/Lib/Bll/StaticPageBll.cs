using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using log4net;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.Setiings;

namespace WebApplication.Lib.Bll
{
    public static class StaticPageBll
    {
        private static readonly string _defaultValue =
            "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus. Vivamus elementum semper nisi. Aenean vulputate eleifend tellus. Aenean leo ligula, porttitor eu, consequat vitae, eleifend ac, enim. Aliquam lorem ante, dapibus in, viverra quis, feugiat a, tellus. Phasellus viverra nulla ut metus varius laoreet. Quisque rutrum. Aenean imperdiet. Etiam ultricies nisi vel augue. Curabitur ullamcorper ultricies nisi. Nam eget dui. Etiam rhoncus. Maecenas tempus, tellus eget condimentum rhoncus, sem quam semper libero, sit amet adipiscing sem neque sed ipsum. Nam quam nunc, blandit vel, luctus pulvinar, hendrerit id, lorem. Maecenas nec odio et ante tincidunt tempus. Donec vitae sapien ut libero venenatis faucibus. Nullam quis ante. Etiam sit amet orci eget eros faucibus tincidunt. Duis leo. Sed fringilla mauris sit amet nibh. Donec sodales sagittis magna. Sed consequat, leo eget bibendum sodales, augue velit cursus nunc,";

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static class PageTypeName
        {
            public const string AboutUs = "About Us";
            public const string Returns = "Return";
            public const string Operating = "Operating";
            public const string Faq = "FAQ";
            public const string DisputeResolutionPolicy = "Dispute Resolution Policy";
        }

        public static StaticPage GetPage(string strType)
        {
            StaticPage page = null;

            try
            {
                using (var dbContext = new Entities())
                {
                    var articleType =
                        dbContext.ArticleTypes.FirstOrDefault(m => m.Name.Equals(strType.Trim()));
                    if (articleType != null)
                    {
                        if (articleType.Status != StatusEnum.Active)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        articleType = new ArticleType
                        {
                            Name = strType.Trim(),
                            Status = StatusEnum.Active
                        };

                        dbContext.ArticleTypes.Add(articleType);
                        dbContext.SaveChanges();
                    }

                    page =
                        dbContext.StaticPages.FirstOrDefault(
                            m => m.Type == articleType.Id && m.Status == StatusEnum.Active);

                    if (page == null)
                    {
                        page = new StaticPage
                        {
                            Type = articleType.Id,
                            Title = articleType.Name,
                            Content = _defaultValue,
                            Status = StatusEnum.Active
                        };

                        dbContext.StaticPages.Add(page);
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("GetPage", exception);
            }
            return page;
        }

        public static bool Update(ArticleModel articleModel)
        {
            if (articleModel == null)
            {
                Log.Info("Update(). ArticleModel null");
                return false;
            }

            try
            {
                using (var dbContext = new Entities())
                {
                    var articleType =
                        dbContext.ArticleTypes.FirstOrDefault(
                            m => m.Id == articleModel.Type && m.Status == StatusEnum.Active);
                    if (articleType != null)
                    {
                        var page =
                            dbContext.StaticPages.FirstOrDefault(
                                m => m.Type == articleModel.Type && m.Status == StatusEnum.Active);

                        if (page == null)
                        {
                            page = new StaticPage
                            {
                                Type = articleType.Id,
                                Title = articleModel.Title,
                                Content = articleModel.Content,
                                Status = StatusEnum.Active
                            };

                            dbContext.StaticPages.Add(page);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            page.Title = articleModel.Title;
                            page.Content = articleModel.Content;
                            dbContext.SaveChanges();
                        }

                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("Update()", exception);
            }

            return false;
        }
    }
}