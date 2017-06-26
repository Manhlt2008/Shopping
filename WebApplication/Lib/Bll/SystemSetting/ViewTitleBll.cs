using Microsoft.Ajax.Utilities;
using WebApplication.Lib.Dal.ConfigData;

namespace WebApplication.Lib.Bll.SystemSetting
{
    public class ViewTitleBll
    {
        public static string Title(string value)
        {
            var viewTitleConfig = ViewTitleConfig.Instance;

            var title = string.Empty;

            if (!viewTitleConfig.Prefix.IsNullOrWhiteSpace())
            {
                title += string.Format("{0} - ", viewTitleConfig.Prefix);
            }

            title += value;

            if (!viewTitleConfig.Postfix.IsNullOrWhiteSpace())
            {
                title += string.Format(" - {0}", viewTitleConfig.Postfix);
            }

            return title;
        }
    }
}