using System.Collections.Generic;

namespace WebApplication.Lib.Util.Constant
{
    public class OrderStatusEnum
    {
        public const int New = 1;
        public const int Reject = 2;
        public const int Purchased = 3;
        public const int Delevering = 4;
        public const int Completed = 5;
        public const int Deleted = 6;

        public static string GetByStatus(int status)
        {
            switch (status)
            {
                case New:
                    return "Mới";
                case Reject:
                    return "Từ chối";
                case Purchased:
                    return "Đã thanh toán";
                case Delevering:
                    return "Đang vận chuyển";
                case Completed:
                    return "Hoàn tất";
                case Deleted:
                    return "Đã xóa";
                default:
                    return string.Empty;
            }
        }
    }
}