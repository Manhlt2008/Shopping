namespace WebApplication.Lib.Util.Constant
{
    public class StatusEnum
    {
        public const int Active = 1;
        public const int InActive = 2;
        public const int Pending = 3;

        public const int CombineActivePending = 1300;

        public static string GetCombineStatus(int status)
        {
            switch (status)
            {
                case CombineActivePending:
                    return "1,3";
                default:
                    return status.ToString();
            }
        }

        public enum PaymentMethodEnum
        {
            Unpaid = 0,
            Cod = 1,
            A123Pay = 2
        }

        public static class ReviewEnum
        {
            public static class Status
            {
                public const int New = 1;
                public const int Approve = 2;
                public const int Denied = 3;
                public const int Deleted = 4;

                public static bool IsValid(int status)
                {
                    return (status == New || status == Approve || status == Denied || status == Deleted);
                }
            }

            public static class Action
            {
                public const int New = 1;
                public const int Approve = 2;
            }
        }

        public static class OrderEnum
        {
            public static class OrderBySetting
            {
                /// <summary>
                /// Order by created order date
                /// </summary>
                public const int Default = 1;

                public const int OrderCode = 2;

                public const int New = 3;

                public const int Completed = 4;

                public const int Reject = 5;

                public const int Purchased = 6;

                public const int Delevering = 7;

                public const int All = 8;

            }
        }
    }
}