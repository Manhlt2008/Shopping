using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Lib.Bll.ApiHelper.DealToDay
{
    public static partial class DealToDayBll
    {
        public enum StatusCodeEnum
        {
            Success = 0,
            SystemError = 1,
            MissingOrInvalidParams = 2,
            UnknowPartner = 3,
            ExceptionError = 4,
            PartnerSuspended = 5,
            DealExpireOrUnapprove = 6,
            OutOfDealNumber = 7,
            InvalidSignuture = 8,
            NotExistedQuery = 10
        }
    }
}