namespace WebApplication.Lib.Util.Constant
{
    public class MessageEnum
    {
        public const string ApplicationName = "Phụ Nữ Mart";
        public static string GeneratePageTitle(string title)
        {
            if (title == null || title.Trim().Equals(string.Empty))
            {
                title = "Trang chủ";
            }
            return string.Format("{0} - {1}", title, ApplicationName);
        }
        public static class OrderMessage
        {
            public static class Save
            {
                public const string TitleCreate = "Tạo mới hóa đơn";
                public const string TitleUpdate = "Cập nhật hóa đơn";
                public const string Create = "Tạo mới hóa đơn";
                public const string UpdateInfo = "Cập nhật hóa đơn";

                public const string OrderTitle = "Thông Tin Hóa Đơn";
                public const string OrderId = "ID Hóa Đơn";
                public const string OrderCode = "Mã Hóa Đơn";
                public const string OrderStatus = "Trạng Thái";

                public const string ClientDetail = "Thông Tin Người Đăt Hàng";
                public const string ClientDetailName = "Họ và Tên";
                public const string ClientDetailEmail= "Email";
                public const string ClientDetailPhone= "Số Điện Thoại";

                public const string ClientItems = "Danh Mục Hàng";
                public const string ClientItemsAdd = "Thêm Sản Phẩm";
                public const string ClientItemsProductName = "Tên Sản Phẩm";
                public const string ClientItemsCategory = "Danh Mục Sản Phẩm";
                public const string ClientItemsUnitPrice = "Đơn Giá";
                public const string ClientItemsQuantity = "Số Lượng";
                public const string ClientItemsDiscount = "Chiết Khấu";
                public const string ClientItemsAmount = "Thành Tiền";
                public const string ClientItemsRemoveItem = "Xóa Sản Phẩm";
                public const string ClientItemsDiscardChange = "Hủy Thay Đổi";
                public const string ClientItemsUpdateOrder = "Lưu";
                
                public const string AddProductModalTitle = "Thêm Sản Phẩm";
                public const string AddProductModalProduct = "Sản Phẩm";
                public const string AddProductModalClose = "Hủy";
                public const string AddProductModalAdd = "Thêm";

            }
        }

        public static class Breadcrumbs
        {
            public static string Manage(string title)
            {
                return string.Format("Quản lý {0}", title);
            }

            public static class OrderBreadcrumb
            {
                public const string Manage = "đơn hàng";
            }

            public static class SettingBreadcrumb
            {
                public const string Setting = "Cấu Hình";

                public const string Index = "Cấu Hình Chung";

                public const string Footer = "Cấu Hình Footer";

                public const string ContacUs = "Cấu Hình Contact Us";

                public const string Order = "Câu Hình Order";

            }
        }
    }
}