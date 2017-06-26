using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication.Models.Category;
using WebApplication.Models.User;

namespace WebApplication.Models.Supplier
{
    public class Supplier
    {
        
        public long Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [StringLength(200)]
        public string Phone { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Website { get; set; }

        [Required]
        public string Facebook { get; set; }
        public string ExraInfo { get; set; }
        public byte Status { get; set; }
        public UserModel UserModel { get; set; }

        public List<long> CategoryIds { get; set; }

        public List<long> AccountIds { get; set; }

        public CategoryDropDownListModel Categories { get; set; }

        public Supplier()
        {
            UserModel = new UserModel();
            CategoryIds = new List<long>();
            AccountIds = new List<long>();
            Categories = new CategoryDropDownListModel();
        }
    }
}