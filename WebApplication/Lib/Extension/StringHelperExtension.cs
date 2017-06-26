using System;
using System.Web.Mvc;

namespace WebApplication.Lib.Extension
{
    public static class StringHelperExtension
    {
        public static string ToVND(this int priceInput )
        {
            return string.Format("Di");
        }

        public static string ToVND(this HtmlHelper helper)
        {
            return string.Format("Di1");
        }

        public static string ToVND(this String sysString)
        {
            return string.Format("Di2");
        }
    }
}