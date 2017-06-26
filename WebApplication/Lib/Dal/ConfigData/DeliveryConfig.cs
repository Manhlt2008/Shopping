using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Lib.Bll;

namespace WebApplication.Lib.Dal.ConfigData
{
    public class DeliveryConfig
    {
        private static DeliveryConfig _instance;
        public string DeliveryApiToken { get; set; }
        public string DeliveryCreateOrderPostUrl { get; set; }
        public string DeliveryCreateOrderPostUrlTest { get; set; }
        public string DeliveryCheckStatusOrderPostUrl { get; set; }
        public string DeliveryCheckStatusOrderPostUrlTest { get; set; }
        public string DeliveryCancelOrderPostUrl { get; set; }
        public string DeliveryCancelOrderPostUrlTest { get; set; }
        public string DeliveryUpdateTrackingOrderPostUrl { get; set; }
        public string DeliveryUpdateTrackingOrderPostUrlTest { get; set; }
        public string DeliveryPushOrderInfoPostUrl { get; set; }
        public string DeliveryPushOrderInfoPostUrlTest { get; set; }
        public string DeliveryPriceCalculatorUrl { get; set; }
        public DeliveryConfig()
        {
            var settings = SettingsBll.GetSettings(SettingsBll.SettingTypeGroup.Delivery);
            string deliveryapitoken;
            string deliverycreateorderposturl;
            string deliverycreateorderposturltest;
            string deliverycheckstatusorderpost;
            string deliverycheckstatusorderposttest;
            string deliverycancelorderposturl;
            string deliverycancelorderposturltest;
            string deliveryupdatetrackingorderposturl;
            string deliveryupdatetrackingorderposturltest;
            string deliverypushorderinfoposturl;
            string deliverypushorderinfoposturltest;
            string deliverypricecalculatorurl;
            settings.TryGetValue(SettingsBll.SettingNames.DeliveryApiToken, out deliveryapitoken);
            settings.TryGetValue(SettingsBll.SettingNames.DeliveryCreateOrderPostUrl, out deliverycreateorderposturl);
            settings.TryGetValue(SettingsBll.SettingNames.DeliveryCreateOrderPostUrlTest,
                out deliverycreateorderposturltest);
            settings.TryGetValue(SettingsBll.SettingNames.DeliveryCheckStatusOrderPostUrl,
                out deliverycheckstatusorderpost);
            settings.TryGetValue(SettingsBll.SettingNames.DeliveryCheckStatusOrderPostUrlTest,
                out deliverycheckstatusorderposttest);
            settings.TryGetValue(SettingsBll.SettingNames.DeliveryCancelOrderPostUrl, out deliverycancelorderposturl);
            settings.TryGetValue(SettingsBll.SettingNames.DeliveryCancelOrderPostUrlTest,
                out deliverycancelorderposturltest);
            settings.TryGetValue(SettingsBll.SettingNames.DeliveryUpdateTrackingOrderPostUrl,
                out deliveryupdatetrackingorderposturl);
            settings.TryGetValue(SettingsBll.SettingNames.DeliveryUpdateTrackingOrderPostUrlTest,
                out deliveryupdatetrackingorderposturltest);
            settings.TryGetValue(SettingsBll.SettingNames.DeliveryPushOrderInfoPostUrl, out deliverypushorderinfoposturl);
            settings.TryGetValue(SettingsBll.SettingNames.DeliveryPushOrderInfoPostUrlTest,
                out deliverypushorderinfoposturltest);
            settings.TryGetValue(SettingsBll.SettingNames.DeliveryPriceCalculator, out deliverypricecalculatorurl);
            DeliveryApiToken = deliveryapitoken;
            DeliveryCreateOrderPostUrl = deliverycreateorderposturl;
            DeliveryCreateOrderPostUrlTest = deliverycreateorderposturltest;
            DeliveryCheckStatusOrderPostUrl = deliverycheckstatusorderpost;
            DeliveryCheckStatusOrderPostUrlTest = deliverycheckstatusorderposttest;
            DeliveryCancelOrderPostUrl = deliverycancelorderposturl;
            DeliveryCancelOrderPostUrlTest = deliverycancelorderposturltest;
            DeliveryUpdateTrackingOrderPostUrl = deliveryupdatetrackingorderposturl;
            DeliveryUpdateTrackingOrderPostUrlTest = deliveryupdatetrackingorderposturltest;
            DeliveryPushOrderInfoPostUrl = deliverypushorderinfoposturl;
            DeliveryPushOrderInfoPostUrlTest = deliverypushorderinfoposturltest;
            DeliveryPriceCalculatorUrl = deliverypricecalculatorurl;
        }

        public static DeliveryConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DeliveryConfig();
                }
                return _instance;
            }
        }
    }
}