using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models.User;

namespace WebApplication.Models.Slider
{
    public class SliderModel : ResultModel
    {
        [Display(Name = "Id")]
        public long Id { get; set; }

        [Display(Name = "Hình ảnh")]
        public HttpPostedFileBase Image { get; set; }

        [Display(Name = "Hình ảnh")]
        public string Image_Url { get; set; }

        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Display(Name = "Mô tả")]
        [AllowHtml]
        public string Description { get; set; }

        [Display(Name = "Đường dẫn")]
        public string GoToLink { get; set; }

        [Display(Name = "Loại hiển thị")]
        public string Type { get; set; }

        [Display(Name = "Trạng thái")]
        public int Status { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Thứ tự")]
        public int IOrder { get; set; }
    }
}