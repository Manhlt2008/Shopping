using WebApplication.Models.User;

namespace WebApplication.Lib.Util.Constant
{
    public class Result
    {
        public static ResultModel SUCCESS = new ResultModel { Code = 0, Message = "Thành công" };
        public static ResultModel FAILED = new ResultModel { Code = 1, Message = "Thất bại" };
        public static ResultModel SYSTEM = new ResultModel { Code = -1, Message = "Lỗi hệ thống" };
        public static ResultModel DATABASE = new ResultModel { Code = 2, Message = "Lỗi database" };
        public static ResultModel AUTH = new ResultModel { Code = 3, Message = "Lỗi chứng thực" };

        // Account
        public static ResultModel ACCOUNT_INVALID = new ResultModel { Code = 100, Message = "Tài khoản hoặc mật khẩu không chính xác." };
        public static ResultModel ACCOUNT_LOCKED = new ResultModel { Code = 101, Message = "Tài khoản đã bị khóa." };
        public static ResultModel ACCOUNT_DELETED = new ResultModel { Code = 102, Message = "Tài khoản đã bị xóa." };
        public static ResultModel ACCOUNT_NOT_ADMIN = new ResultModel { Code = 103, Message = "Tài khoản không thuộc nhóm quản trị." };
        public static ResultModel ACCOUNT_NOT_USER = new ResultModel { Code = 104, Message = "Tài khoản không thuộc nhóm member." };
        public static ResultModel ACCOUNT_IS_ADMIN = new ResultModel { Code = 105, Message = "Tài khoản không thuộc nhóm quản trị. Không thể cập nhật" };

        public static ResultModel ACCOUNT_INCORRECT_PASSWORD_OLD = new ResultModel { Code = 204, Message = "Mật khẩu cũ không chính xác" };
        public static ResultModel ACCOUNT_PASSWORD_IS_DIFFERENT = new ResultModel { Code = 204, Message = "Mật khẩu không trùng khớp" };

        public static ResultModel ACCOUNT_FIRSTNAME_IS_EMPTY = new ResultModel { Code = 1001, Message = "Họ không tìm thấy" };
        public static ResultModel ACCOUNT_LASTNAME_IS_EMPTY = new ResultModel { Code = 1002, Message = "Tên không tìm thấy" };
        public static ResultModel ACCOUNT_EMAIL_IS_EMPTY = new ResultModel { Code = 1003, Message = "Email không tìm thấy" };
        public static ResultModel ACCOUNT_PASSWORD_IS_EMPTY = new ResultModel { Code = 1004, Message = "Mật khẩu không tìm thấy" };
        public static ResultModel ACCOUNT_EMAIL_INVALID_FORMAT = new ResultModel { Code = 1005, Message = "Email không đúng định dạng" };
        public static ResultModel ACCOUNT_EMAIL_IS_EXISTED = new ResultModel { Code = 1006, Message = "Email đã tồn tại" };
        public static ResultModel ACCOUNT_EMAIL_IS_NOT_EXISTED = new ResultModel { Code = 1007, Message = "Email không tồn tại" };
        public static ResultModel ACCOUNT_PASSWORD_INVALID_LENGTH = new ResultModel { Code = 1008, Message = "Mật khẩu độ dài không hợp lệ" };

        public static ResultModel ACCOUNT_INCORRECT_TOKEN_FORGOT_PASSWORD = new ResultModel { Code = 1009, Message = "Token hoặc Email không chính xác" };
        public static ResultModel ACCOUNT_TOKEN_EXPIRED = new ResultModel { Code = 1010, Message = "Token đã quá hạn" };
        public static ResultModel ACCOUNT_TOKEN_NOT_FOUND = new ResultModel { Code = 1011, Message = "Token không tìm thấy" };

        public static ResultModel INVALID_DATA = new ResultModel { Code = 1111, Message = "Dữ liệu không hợp lệ" };
        public static ResultModel FAIL_COVER = new ResultModel { Code = 2222, Message = "Lưu hình ảnh thất bại" };
    }
}