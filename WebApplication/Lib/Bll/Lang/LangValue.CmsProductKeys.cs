using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebApplication.Lib.Bll.Lang
{
	public partial class LangValue
	{
		public static class CmsProduct
		{
		    public static class View
		    {
		        public const string Title = "Title";
		        public const string Home = "Home";
		        public const string Information = "Information";
		        public const string Price = "Price";
		        public const string AddToCart = "AddToCart";
		        public const string Category = "Category";
		        public const string Description = "Description";
		        public const string Reviews = "Reviews";
		        public const string ProductDescription = "ProductDescription";
		        public const string ProductRelated = "ProductRelated";
		    }

		    public static class Category
		    {
		        public const string Title = "Title";
		        public const string Home = "Home";
		        public const string FilterBy = "FilterBy";
		        public const string Display = "Display";
		    }
		}
	}
}