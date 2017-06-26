using System;
using System.Reflection.Emit;

namespace WebApplication.Lib.Util.Theme
{
    public partial class ThemeName
    {
        public enum ThemeNames
        {
            Default, Furnishing, ShopMain
        }

        public static class ViewName
        {
            public enum HomeViewNameEnum
            {
                Index
            }

            public enum ProductViewNameEnum
            {
                Index, Category, View, Stand
            }

            public enum AuthenticationViewNameEnum
            {
                Login, Register, ForgotPassword
            }

            public enum ArticleViewNameEnum
            {
                Index
            }

            public enum ErrorViewNameEnum
            {
                PageNotFound
            }

            public enum OrderViewNameEnum
            {
                Cart
            }

            public enum UserViewNameEnum
            {
                Info, ChangePassword
            }

            public enum SettingViewNameEnum
            {
                ContactUs
            }
        }

        private static ThemeNames _themeName = ThemeNames.Default;

        protected internal ThemeName(ThemeNames themeName)
        {
            _themeName = themeName;
        }

        public string MasterPage
        {
            get
            {
                switch (_themeName)
                {
                    case ThemeNames.Furnishing:
                        return "~/Views/Shared/Furnishing/_FurnishingLayout.cshtml";
                    case ThemeNames.ShopMain:
                        return "~/Views/Shared/ShopMain/_ShopMainLayout.cshtml";
                    default:
                        return "~/Views/Shared/Shop/_ShopLayout.cshtml";
                }
            }
        }

        public void SetThemeName(ThemeNames themeNames)
        {
            _themeName = themeNames;
        }

        public static string GetView(Enum viewName)
        {
            switch (_themeName)
            {
                case ThemeNames.Furnishing:
                    return $"Furnishing/{viewName}";
                case ThemeNames.ShopMain:
                    return $"ShopMain/{viewName}";
                default:
                    return viewName.ToString();
            }
        }

        public static string GetView(string controller, string action)
        {
            switch (_themeName)
            {
                case ThemeNames.Furnishing:
                    return string.Format("../{0}/Furnishing/{1}", controller, action);
                case ThemeNames.ShopMain:
                    return string.Format("../{0}/ShopMain/{1}", controller, action);
                default:
                    return string.Format("../{0}/{1}", controller, action);
            }
        }
    }
}