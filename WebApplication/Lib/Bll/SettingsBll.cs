using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;
using log4net;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Models.Setiings;

namespace WebApplication.Lib.Bll
{
    public static class SettingsBll
    {
        private class SettingGroup
        {
            public string Key { get; set; }
            public string Value { get; set; }

            public SettingGroup()
            {
                Value = string.Empty;
                Key = string.Empty;
            }

            public SettingGroup(string name)
            {
                string defaultValue;
                string key;

                SettingKeys.TryGetValue(name, out key);
                DefaultValue.TryGetValue(name, out defaultValue);

                Value = defaultValue;
                Key = key;
            }
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public enum SettingTypeGroup
        {
            Footer,
            ContactUs,
            Payment123Pay,
            Delivery,
            ViewTitle,
            DealToDay,
            Order
        }

        #region [DefaultData]
        public static class SettingNames
        {
            #region [Footer]
            public const string FooterAbout = "Footer.About";
            public const string FooterCopyright = "Footer.Copyright";

            public const string FooterFacebookPagePluginUrl = "Footer.Facebook.PagePluginUrl";
            public const string FooterFacebookPagePluginTabs = "Footer.Facebook.PagePluginTabs";
            public const string FooterFacebookPagePluginWidth = "Footer.Facebook.PagePluginWidth";
            public const string FooterFacebookPagePluginHeight = "Footer.Facebook.PagePluginHeight";
            public const string FooterFacebookPagePluginTitle = "Footer.Facebook.PagePluginTitle";

            public const string FooterFollowUsFacebook = "Footer.FollowUs.Facebook";
            public const string FooterFollowUsTwitter = "Footer.FollowUs.Twitter";
            public const string FollowUsGooglePlus = "Footer.FollowUs.GooglePlus";
            public const string FollowUsYouTube = "Footer.FollowUs.YouTube";
            #endregion

            #region [Contact Us]
            public const string ContactUsAddress = "ContactUs.Address";
            public const string ContactUsTelephone = "ContactUs.Telephone";
            public const string ContactUsFax = "ContactUs.Fax";
            public const string ContactUsCommingTime = "ContactUs.CommingTime";
            public const string ContactUsComments = "ContactUs.Comments";
            public const string ContactUsViewGoogleMap = "ContactUs.ViewGoogleMap";
            public const string ContactUsEmail = "ContactUs.Email";
            #endregion

            #region [Payment123]
            public const string Payment123PayMerchantCode = "Payment123PayMerchantCode";
            public const string Payment123PayPassCode = "Payment123PayPassCode";
            public const string Payment123PaySecretKey = "Payment123PaySecretKey";
            public const string Payment123PayCreateOrderUrl = "Payment123PayCreateOrderUrl";
            public const string Payment123PayQuerryOrderUrl = "Payment123PayQuerryOrderUrl";
            public const string Payment123PayNotifyUrl = "Payment123PayNotifyUrl";
            #endregion

            #region [GHTK]
            public const string DeliveryApiToken = "Delivery.ApiToken";
            public const string DeliveryCreateOrderPostUrl = "Delivery.CreateOrderPostUrl";
            public const string DeliveryCreateOrderPostUrlTest = "Delivery.CreateOrderPostUrlTest";
            public const string DeliveryCheckStatusOrderPostUrl = "Delivery.CheckStatusPostUrl";
            public const string DeliveryCheckStatusOrderPostUrlTest = "Delivery.CheckStatusOrderPostUrlTest";
            public const string DeliveryCancelOrderPostUrl = "Delivery.CancelPostUrl";
            public const string DeliveryCancelOrderPostUrlTest = "Delivery.CancelPostUrlTest";
            public const string DeliveryUpdateTrackingOrderPostUrl = "Delivery.UpdateTrackingOrderPostUrl";
            public const string DeliveryUpdateTrackingOrderPostUrlTest = "Delivery.UpdateTrackingOrderPostUrlTest";
            public const string DeliveryPushOrderInfoPostUrl = "Delivery.PushOrderInfoPostUrl";
            public const string DeliveryPushOrderInfoPostUrlTest = "Delivery.PushOrderInfoPostUrlTest";
            public const string DeliveryPriceCalculator = "Delivery.DeliveryPriceCalculator";
            #endregion

            #region [ViewTitle]
            public const string ViewTitlePrefix = "ViewTitle.Prefix";
            public const string ViewTitlePostfix = "ViewTitle.Postfix";
            #endregion

            #region [DealToDay]
            public const string DealToDayPartnerCode = "DealToDay.PartnerCode";
            public const string DealToDaySignature = "DealToDay.Signature";
            public const string DealToDayServiceUrl = "DealToDay.ServiceUrl";
            #endregion

            #region [Order]
            public const string OrderLimitedOfEachDay = "Order.LimitedOfEachDay";
            #endregion
        }

        public static Dictionary<string, string> SettingKeys = new Dictionary<string, string>
        {
            #region [Footer]
            {SettingNames.FooterAbout, SettingNames.FooterAbout},
            {SettingNames.FooterCopyright, SettingNames.FooterCopyright},

            {SettingNames.FooterFacebookPagePluginUrl, SettingNames.FooterFacebookPagePluginUrl},
            {SettingNames.FooterFacebookPagePluginTabs, SettingNames.FooterFacebookPagePluginTabs},
            {SettingNames.FooterFacebookPagePluginWidth, SettingNames.FooterFacebookPagePluginWidth},
            {SettingNames.FooterFacebookPagePluginHeight, SettingNames.FooterFacebookPagePluginHeight},
            {SettingNames.FooterFacebookPagePluginTitle, SettingNames.FooterFacebookPagePluginTitle},

            {SettingNames.FooterFollowUsFacebook, SettingNames.FooterFollowUsFacebook},
            {SettingNames.FooterFollowUsTwitter, SettingNames.FooterFollowUsTwitter},
            {SettingNames.FollowUsGooglePlus, SettingNames.FollowUsGooglePlus},
            {SettingNames.FollowUsYouTube, SettingNames.FollowUsYouTube},
            #endregion

            #region [Contact Us]
            {SettingNames.ContactUsAddress, SettingNames.ContactUsAddress},
            {SettingNames.ContactUsTelephone, SettingNames.ContactUsTelephone},
            {SettingNames.ContactUsFax, SettingNames.ContactUsFax},
            {SettingNames.ContactUsCommingTime, SettingNames.ContactUsCommingTime},
            {SettingNames.ContactUsComments, SettingNames.ContactUsComments},
            {SettingNames.ContactUsViewGoogleMap, SettingNames.ContactUsViewGoogleMap},
            {SettingNames.ContactUsEmail, SettingNames.ContactUsEmail},
            #endregion

            #region [GHTK]
            {SettingNames.DeliveryApiToken, SettingNames.DeliveryApiToken },
            {SettingNames.DeliveryCreateOrderPostUrl, SettingNames.DeliveryCreateOrderPostUrl },
            {SettingNames.DeliveryCreateOrderPostUrlTest,SettingNames.DeliveryCreateOrderPostUrlTest },
            {SettingNames.DeliveryCheckStatusOrderPostUrl,SettingNames.DeliveryCheckStatusOrderPostUrl },
            {SettingNames.DeliveryCheckStatusOrderPostUrlTest,SettingNames.DeliveryCheckStatusOrderPostUrlTest },
            {SettingNames.DeliveryCancelOrderPostUrl,SettingNames.DeliveryCancelOrderPostUrl },
            {SettingNames.DeliveryCancelOrderPostUrlTest,SettingNames.DeliveryCancelOrderPostUrlTest },
            {SettingNames.DeliveryUpdateTrackingOrderPostUrl,SettingNames.DeliveryUpdateTrackingOrderPostUrl },
            {SettingNames.DeliveryUpdateTrackingOrderPostUrlTest,SettingNames.DeliveryUpdateTrackingOrderPostUrlTest },
            {SettingNames.DeliveryPushOrderInfoPostUrl,SettingNames.DeliveryPushOrderInfoPostUrl },
            {SettingNames.DeliveryPushOrderInfoPostUrlTest,SettingNames.DeliveryPushOrderInfoPostUrlTest },
            {SettingNames.DeliveryPriceCalculator,SettingNames.DeliveryPriceCalculator },
            #endregion

            #region [Payment123]
            {SettingNames.Payment123PayMerchantCode, SettingNames.Payment123PayMerchantCode},
            {SettingNames.Payment123PayCreateOrderUrl, SettingNames.Payment123PayCreateOrderUrl},
            {SettingNames.Payment123PayNotifyUrl, SettingNames.Payment123PayNotifyUrl},
            {SettingNames.Payment123PayPassCode, SettingNames.Payment123PayPassCode},
            {SettingNames.Payment123PaySecretKey, SettingNames.Payment123PaySecretKey},
            {SettingNames.Payment123PayQuerryOrderUrl, SettingNames.Payment123PayQuerryOrderUrl},
            #endregion

            #region [ViewTitle]
            {SettingNames.ViewTitlePrefix, SettingNames.ViewTitlePrefix },
            {SettingNames.ViewTitlePostfix, SettingNames.ViewTitlePostfix},
            #endregion

            #region [DealToDay]
            {SettingNames.DealToDayPartnerCode, SettingNames.DealToDayPartnerCode },
            {SettingNames.DealToDaySignature, SettingNames.DealToDaySignature },
            {SettingNames.DealToDayServiceUrl, SettingNames.DealToDayServiceUrl },
            #endregion
            
            #region [DealToDay]
            {SettingNames.OrderLimitedOfEachDay, SettingNames.OrderLimitedOfEachDay }
            #endregion
        };

        public static Dictionary<string, string> DefaultValue = new Dictionary<string, string>
        {
            #region [Footer]
            {SettingNames.FooterAbout, "Comfort is a very important thing nowadays because it is a condition of satisfaction and calmness. It is clear that our way of life must be as comfortable as possible. Home electronics satisfy our wishes and make our life more pleasant. We must admit that our way of life depends on quality of different goods of popular brands. Many of our clients were surprised by the incredible assortment of products in our store. You know, we have many devoted customers all over the world, and this fact proves that we sell only quality commodities. Recipe of our success is a fair price and premium quality."},
            {SettingNames.FooterCopyright, "Powered By <a href=\"http://inlamia.in/\">Inlamia</a><br /> Group &copy; 2016"},

            {SettingNames.FooterFacebookPagePluginUrl,"https://www.facebook.com/1011902158858219/"},
            {SettingNames.FooterFacebookPagePluginTabs, "timeline"},
            {SettingNames.FooterFacebookPagePluginWidth, "500"},
            {SettingNames.FooterFacebookPagePluginHeight, "150"},
            {SettingNames.FooterFacebookPagePluginTitle, "Văn Phân Mảnh"},

            {SettingNames.FooterFollowUsFacebook, string.Empty},
            {SettingNames.FooterFollowUsTwitter,string.Empty},
            {SettingNames.FollowUsGooglePlus, string.Empty},
            {SettingNames.FollowUsYouTube, string.Empty},


            {SettingNames.ContactUsAddress, string.Empty},
            {SettingNames.ContactUsTelephone, string.Empty},
            {SettingNames.ContactUsFax, string.Empty},
            {SettingNames.ContactUsCommingTime, string.Empty},
            {SettingNames.ContactUsComments, string.Empty},
            {SettingNames.ContactUsViewGoogleMap, string.Empty},
            {SettingNames.ContactUsEmail, string.Empty},
            #endregion

            #region [123Pay]
            {SettingNames.Payment123PayMerchantCode, "MICODE01"},
            {SettingNames.Payment123PayPassCode, "MIPASSCODE"},
            {SettingNames.Payment123PaySecretKey, "MIKEY"},
            {SettingNames.Payment123PayCreateOrderUrl, "https://sandbox.123pay.vn/miservice/createOrder1"},
            {SettingNames.Payment123PayQuerryOrderUrl, "https://sandbox.123pay.vn/miservice/queryOrder1"},
            {SettingNames.Payment123PayNotifyUrl, string.Empty},
            #endregion
            
            #region[GHTK]
            {SettingNames.DeliveryApiToken,"3B32E14478009b4CaeB052aD6bEa50542759383f"},
            {SettingNames.DeliveryCreateOrderPostUrl,"http://services.giaohangtietkiem.vn/services/orders/add" },
            {SettingNames.DeliveryCreateOrderPostUrlTest,"http://dev.giaohangtietkiem.vn/services/orders/add" },
            {SettingNames.DeliveryCheckStatusOrderPostUrl,"http://services.giaohangtietkiem.vn/services/orders/status" },
            {SettingNames.DeliveryCheckStatusOrderPostUrlTest ,"http://dev.giaohangtietkiem.vn/services/orders/status"},
            {SettingNames.DeliveryCancelOrderPostUrl,"http://services.giaohangtietkiem.vn/services/orders/update" },
            {SettingNames.DeliveryCancelOrderPostUrlTest,"http://dev.giaohangtietkiem.vn/services/orders/update" },
            {SettingNames.DeliveryUpdateTrackingOrderPostUrl,"http://services.giaohangtietkiem.vn/services/order/update_tracking" },
            {SettingNames.DeliveryUpdateTrackingOrderPostUrlTest,"http://dev.giaohangtietkiem.vn/services/order/update_tracking" },
            {SettingNames.DeliveryPushOrderInfoPostUrl,string.Empty },
            {SettingNames.DeliveryPushOrderInfoPostUrlTest,string.Empty },
            {SettingNames.DeliveryPriceCalculator,"http://dev.giaohangtietkiem.vn/services/orders/fee" },
            #endregion

            #region [View Title]
            {SettingNames.ViewTitlePrefix, "" },
            {SettingNames.ViewTitlePostfix, "Phụ Nữ Mart Online" },
            #endregion

            #region [DealToDay]
            {SettingNames.DealToDayPartnerCode, "PHUNUMART" },
            {SettingNames.DealToDaySignature, "3d6517f0144d3d85a9bf2cbd1f287323b7bb8a2e" },
            {SettingNames.DealToDayServiceUrl, "http://service.dealtoday.vn/processrequest" },
            #endregion

            #region [Order]
            {SettingNames.OrderLimitedOfEachDay, "10" }
            #endregion
        };
        #endregion

        #region [Get Settings]
        public static string[] GetSettingNameGroup(SettingTypeGroup settingTypeGroup)
        {
            switch (settingTypeGroup)
            {
                case SettingTypeGroup.Footer:
                    return new[]
                    {
                        SettingNames.FooterAbout,
                        SettingNames.FooterCopyright,

                        SettingNames.FooterFacebookPagePluginUrl,
                        SettingNames.FooterFacebookPagePluginTabs,
                        SettingNames.FooterFacebookPagePluginWidth,
                        SettingNames.FooterFacebookPagePluginHeight,
                        SettingNames.FooterFacebookPagePluginTitle,

                        SettingNames.FooterFollowUsFacebook,
                        SettingNames.FooterFollowUsTwitter,
                        SettingNames.FollowUsGooglePlus,
                        SettingNames.FollowUsYouTube

                    };
                case SettingTypeGroup.ContactUs:
                    return new[]
                    {
                        SettingNames.ContactUsAddress,
                        SettingNames.ContactUsTelephone,
                        SettingNames.ContactUsFax,
                        SettingNames.ContactUsCommingTime,
                        SettingNames.ContactUsComments,
                        SettingNames.ContactUsViewGoogleMap,
                        SettingNames.ContactUsEmail

                    };
                case SettingTypeGroup.Payment123Pay:
                    return new[]
                    {
                        SettingNames.Payment123PayMerchantCode,
                        SettingNames.Payment123PayPassCode,
                        SettingNames.Payment123PaySecretKey,
                        SettingNames.Payment123PayCreateOrderUrl,
                        SettingNames.Payment123PayQuerryOrderUrl,
                        SettingNames.Payment123PayNotifyUrl
                    };
                case SettingTypeGroup.Delivery:
                    return new[]
                    {
                        SettingNames.DeliveryApiToken,
                        SettingNames.DeliveryCreateOrderPostUrl,
                        SettingNames.DeliveryCreateOrderPostUrlTest,
                        SettingNames.DeliveryCheckStatusOrderPostUrl,
                        SettingNames.DeliveryCheckStatusOrderPostUrlTest,
                        SettingNames.DeliveryCancelOrderPostUrl,
                        SettingNames.DeliveryCancelOrderPostUrlTest,
                        SettingNames.DeliveryUpdateTrackingOrderPostUrl,
                        SettingNames.DeliveryUpdateTrackingOrderPostUrlTest,
                        SettingNames.DeliveryPushOrderInfoPostUrl,
                        SettingNames.DeliveryPushOrderInfoPostUrlTest
                    };
                case SettingTypeGroup.ViewTitle:
                    return new[]
                    {
                        SettingNames.ViewTitlePrefix,
                        SettingNames.ViewTitlePostfix
                    };
                case SettingTypeGroup.DealToDay:
                    return new[]
                    {
                        SettingNames.DealToDayPartnerCode,
                        SettingNames.DealToDaySignature,
                        SettingNames.DealToDayServiceUrl
                    };
                case SettingTypeGroup.Order:
                    return new[]
                    {
                        SettingNames.OrderLimitedOfEachDay
                    };
                default:
                    return new string[0];
            }
        }

        private static Setting GetSetting(ref Entities dbContext, string settingName, string customValue = "")
        {
            try
            {
                SettingGroup settingGroup = new SettingGroup(settingName);

                var setting = dbContext.Settings.FirstOrDefault(m => m.Name.Equals(settingGroup.Key));

                if (setting == null)
                {
                    setting = new Setting
                    {
                        Name = settingGroup.Key,
                        Value = customValue.Trim().Equals(string.Empty) ? settingGroup.Value : customValue.Trim()
                    };

                    dbContext.Settings.Add(setting);
                    dbContext.SaveChanges();
                }

                return setting;

            }
            catch (Exception exception)
            {
                Log.Error("GetSetting(), Setting Name = " + settingName, exception);
            }
            return null;
        }

        public static Dictionary<string, string> GetSettings(SettingTypeGroup settingTypeGroup)
        {
            return GetSettings(GetSettingNameGroup(settingTypeGroup));
        }

        public static Dictionary<string, string> GetSettings(params string[] settingNames)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            try
            {
                using (var dbContext = new Entities())
                {
                    var context = dbContext;
                    using (var scope = new TransactionScope())
                    {
                        var settings = settingNames.Select(settingName => GetSetting(ref context, settingName)).ToList();

                        foreach (var setting in settings)
                        {
                            result.Add(setting.Name, setting.Value);
                        }

                        scope.Complete();
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("GetSettings()", exception);
            }

            return result;
        }
        #endregion

        #region [Footer]
        public static bool SaveFooter(FooterModel model)
        {
            if (model == null)
            {
                return false;
            }

            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        var context = dbContext;

                        var settingNames = GetSettingNameGroup(SettingTypeGroup.Footer).ToList();

                        var settings =
                            dbContext.Settings.Where(m => settingNames.Contains(m.Name)).ToList();

                        foreach (var settingName in settingNames)
                        {
                            var obj = settings.FirstOrDefault(m => m.Name.Equals(settingName));
                            string value;
                            DefaultValue.TryGetValue(settingName, out value);

                            switch (settingName)
                            {
                                case SettingNames.FooterAbout:
                                    value = model.About;
                                    break;
                                case SettingNames.FooterFacebookPagePluginHeight:
                                    value = model.FacebookHeight;
                                    break;
                                case SettingNames.FooterFacebookPagePluginTabs:
                                    value = model.FacebookTabs;
                                    break;
                                case SettingNames.FooterFacebookPagePluginWidth:
                                    value = model.FacebookWidth;
                                    break;
                                case SettingNames.FooterCopyright:
                                    value = model.Copyright;
                                    break;
                                case SettingNames.FooterFacebookPagePluginUrl:
                                    value = model.FacebookUrl;
                                    break;
                                case SettingNames.FooterFacebookPagePluginTitle:
                                    value = model.FacebookTitle;
                                    break;
                                case SettingNames.FooterFollowUsFacebook:
                                    value = model.FollowUsFacebook;
                                    break;
                                case SettingNames.FooterFollowUsTwitter:
                                    value = model.FollowUsTwitter;
                                    break;
                                case SettingNames.FollowUsGooglePlus:
                                    value = model.FollowUsGooglePlus;
                                    break;
                                case SettingNames.FollowUsYouTube:
                                    value = model.FollowUsYouTube;
                                    break;
                                default:
                                    value = string.Empty;
                                    break;
                            }

                            if (obj == null)
                            {
                                obj = GetSetting(ref context, settingName, value);
                            }
                            else
                            {
                                obj.Value = value;
                                dbContext.SaveChanges();
                            }
                        }

                        scope.Complete();

                        return true;

                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("SaveFooter()", exception);
            }

            return false;
        }

        #endregion

        #region [Contact Us]

        public static bool SaveContactUs(ContactUsModel model)
        {
            if (model == null)
            {
                return false;
            }

            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        var context = dbContext;

                        var settingNames = GetSettingNameGroup(SettingTypeGroup.ContactUs).ToList();

                        var settings =
                            dbContext.Settings.Where(m => settingNames.Contains(m.Name)).ToList();

                        foreach (var settingName in settingNames)
                        {
                            var obj = settings.FirstOrDefault(m => m.Name.Equals(settingName));
                            string value;
                            DefaultValue.TryGetValue(settingName, out value);

                            switch (settingName)
                            {

                                case SettingNames.ContactUsAddress:
                                    value = model.ContactUsAddress;
                                    break;
                                case SettingNames.ContactUsComments:
                                    value = model.ContactUsComments;
                                    break;
                                case SettingNames.ContactUsCommingTime:
                                    value = model.ContactUsCommingTime;
                                    break;
                                case SettingNames.ContactUsEmail:
                                    value = model.ContactUsEmail;
                                    break;
                                case SettingNames.ContactUsTelephone:
                                    value = model.ContactUsTelephone;
                                    break;
                                case SettingNames.ContactUsViewGoogleMap:
                                    value = model.ContactUsViewGoogleMap;
                                    break;
                                case SettingNames.ContactUsFax:
                                    value = model.ContactUsFax;
                                    break;
                                default:
                                    value = string.Empty;
                                    break;
                            }

                            if (obj == null)
                            {
                                obj = GetSetting(ref context, settingName, value);
                            }
                            else
                            {
                                obj.Value = value;
                                dbContext.SaveChanges();
                            }
                        }

                        scope.Complete();

                        return true;

                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("SaveFooter()", exception);
            }

            return false;
        }

        #endregion

        #region [Order]
        public static bool SaveOrder(OrderModel model)
        {
            if (model == null)
            {
                return false;
            }

            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        var context = dbContext;

                        var settingNames = GetSettingNameGroup(SettingTypeGroup.Order).ToList();

                        var settings =
                            dbContext.Settings.Where(m => settingNames.Contains(m.Name)).ToList();

                        foreach (var settingName in settingNames)
                        {
                            var obj = settings.FirstOrDefault(m => m.Name.Equals(settingName));
                            string value;
                            DefaultValue.TryGetValue(settingName, out value);

                            switch (settingName)
                            {
                                case SettingNames.OrderLimitedOfEachDay:
                                    value = model.NumberOfOrderEachDay;
                                    break;
                                default:
                                    value = string.Empty;
                                    break;
                            }

                            if (obj == null)
                            {
                                obj = GetSetting(ref context, settingName, value);
                            }
                            else
                            {
                                obj.Value = value;
                                dbContext.SaveChanges();
                            }
                        }

                        scope.Complete();

                        return true;

                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("SaveFooter()", exception);
            }

            return false;
        }

        #endregion
    }
}