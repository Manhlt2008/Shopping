using WebApplication.Lib.Bll;

namespace WebApplication.Lib.Dal.ConfigData
{
    public class ViewTitleConfig
    {
        private static ViewTitleConfig _instance;

        public string Prefix { get; set; }

        public string Postfix { get; set; }

        private ViewTitleConfig()
        {
            var settings = SettingsBll.GetSettings(SettingsBll.SettingTypeGroup.ViewTitle);

            string prefix;
            string postfix;

            settings.TryGetValue(SettingsBll.SettingNames.ViewTitlePrefix, out prefix);
            settings.TryGetValue(SettingsBll.SettingNames.ViewTitlePostfix, out postfix);

            Prefix = prefix;
            Postfix = postfix;
        }

        public static ViewTitleConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance =  new ViewTitleConfig();
                }
                return _instance;
            }
        }
    }
}