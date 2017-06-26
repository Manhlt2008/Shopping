using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace WebApplication.Models.Category
{
    public class CategoryModel : ISerializable
    {
        public long Id { get; set; }

        [Required]
        public long ParentCategoryId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        public CategoryModel() { }
        public CategoryModel(Lib.Dal.DbContext.Category category)
        {
            if (category == null) return;

            Name = category.Name;
            Id = category.Id;
            ParentCategoryId = category.ParentCategoryId ?? 0;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("Name", Name);
            info.AddValue("ParentCategoryId", ParentCategoryId);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            GetObjectData(info, context);
        }
    }
}