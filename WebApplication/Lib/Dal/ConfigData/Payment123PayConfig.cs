using WebApplication.Lib.Bll;

namespace WebApplication.Lib.Dal.ConfigData
{
    public class Payment123PayConfig
    {
        private static Payment123PayConfig _instance;

        public string MerchantCode { get; set; }
        public string PassCode { get; set; }
        public string SecretKey { get; set; }
        public string CreateOrderUrl { get; set; }
        public string QuerryOrderUrl { get; set; }
        public string NotifyUrl { get; set; }
        public string CancelUrl { get; set; }

        private Payment123PayConfig()
        {
            var settings = SettingsBll.GetSettings(SettingsBll.SettingTypeGroup.Payment123Pay);

            string passCode;
            string secretKey;
            string createOrderUrl;
            string querryOrderUrl;
            string merchantCode;
            string notifyUrl;

            settings.TryGetValue(SettingsBll.SettingNames.Payment123PayMerchantCode, out merchantCode);
            settings.TryGetValue(SettingsBll.SettingNames.Payment123PayPassCode, out passCode);
            settings.TryGetValue(SettingsBll.SettingNames.Payment123PaySecretKey, out secretKey);
            settings.TryGetValue(SettingsBll.SettingNames.Payment123PayCreateOrderUrl, out createOrderUrl);
            settings.TryGetValue(SettingsBll.SettingNames.Payment123PayQuerryOrderUrl, out querryOrderUrl);
            settings.TryGetValue(SettingsBll.SettingNames.Payment123PayNotifyUrl, out notifyUrl);

            MerchantCode = merchantCode;
            PassCode = passCode;
            SecretKey = secretKey;
            CreateOrderUrl = createOrderUrl;
            QuerryOrderUrl = querryOrderUrl;
            NotifyUrl = notifyUrl;
        }

        public static Payment123PayConfig Instance {
            get
            {
                if (_instance == null)
                {
                    _instance = new Payment123PayConfig();
                }
                return _instance;
            }
        }

    }
}