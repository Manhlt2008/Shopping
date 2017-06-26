using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Lib.Bll.Lang
{
    public partial class LangValue
    {
        public static class CmsOrder
        {
            public static class Cart
            {
                public const string Title = "Title";
                public const string ShoppingCart = "ShoppingCart";
                public const string CartSubTotal = "CartSubTotal";
                public const string Tax = "Tax";
                public const string CartTotal = "CartTotal";
                public const string Home = "Home";

                public const string OrderTableHeadItem = "OrderTable.Head.Item";
                public const string OrderTableHeadPrice = "OrderTable.Head.Price";
                public const string OrderTableHeadQuantity = "OrderTable.Head.Quantity";
                public const string OrderTableHeadTotal = "OrderTable.Head.Total";

                public const string ProcessCheckout = "ProcessCheckout";
            }
        }
    }
}