using System.Collections.Generic;
using System.Linq;

namespace WebApplication.Models.Category
{
    public class CategoryManageList
    {
        public string Name { get; set; }

        public long Id { get; set; }

        public List<CategoryManageList> CategoryManageLists { get; set; }

        public CategoryManageList ParentCategory { get; set; }

        public CategoryManageList()
        {
            Id = 0;
            Name = string.Empty;
            CategoryManageLists = Enumerable.Empty<CategoryManageList>().ToList();
            ParentCategory = null;
        }
        public CategoryManageList(Lib.Dal.DbContext.Category category)
        {
            if (category == null) return;

            Name = category.Name;
            Id = category.Id;

            CategoryManageLists = new List<CategoryManageList>();

            try
            {
                if (category.Category2 != null)
                {
                    ParentCategory = new CategoryManageList
                    {
                        Id = category.Category2.Id,
                        Name = category.Category2.Name,
                        CategoryManageLists = Enumerable.Empty<CategoryManageList>().ToList(),
                        ParentCategory = null
                    };
                }
            }
            catch
            {
                ParentCategory = null;
            }

            try
            {
                if (!category.ParentCategoryId.Equals(null)) return;
                // Get 1 level only.
                foreach (var category1 in category.Category1)
                {
                    CategoryManageLists.Add(new CategoryManageList(category1));
                }
            }
            catch { }
        }
    }
}