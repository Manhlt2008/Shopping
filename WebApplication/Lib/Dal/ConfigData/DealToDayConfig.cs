using WebApplication.Lib.Bll;

namespace WebApplication.Lib.Dal.ConfigData
{
    public class DealToDayConfig
    {
        private static DealToDayConfig _instance;

        public string PartnerCode { get; set; }
        public string Signature { get; set; }
        public string ServiceUrl { get; set; }

        private DealToDayConfig()
        {
            var settings = SettingsBll.GetSettings(SettingsBll.SettingTypeGroup.DealToDay);

            string partnerCode;
            string signature;
            string serviceUrl;

            settings.TryGetValue(SettingsBll.SettingNames.DealToDayPartnerCode, out partnerCode);
            settings.TryGetValue(SettingsBll.SettingNames.DealToDaySignature, out signature);
            settings.TryGetValue(SettingsBll.SettingNames.DealToDayServiceUrl, out serviceUrl);

            PartnerCode = partnerCode;
            Signature = signature;
            ServiceUrl = serviceUrl;
        }

        public static DealToDayConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DealToDayConfig();
                }
                return _instance;
            }
        }
    }
}