using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Models.Category
{
    public class CategoryDropDownListModel
    {
        public List<SelectListItem> SelectListItems { get; set; }
        public CategoryDropDownListModel()
        {
            SelectListItems = new List<SelectListItem>();
        }

        public CategoryDropDownListModel(List<Lib.Dal.DbContext.Category> categories)
        {
            SelectListItems = new List<SelectListItem>();

            if (categories == null) return;

            foreach (var category in categories)
            {
                SelectListItems.Add(new SelectListItem
                {
                    Selected = false,
                    Text = category.Name,
                    Value = category.Id.ToString()
                });
            }
        }
    }
}