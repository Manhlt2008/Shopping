using System;

namespace WebApplication.Lib.Bll.Payments
{
    public static partial class A123PayBll
    {
        public enum ErrorCodeEnum
        {
            UnknownErrorCode = 0,
            NoError = 1,
            InvalidChecksum = 2,
            SystemBusy = 5000,
            AuthenticateFailure = 6000,
            InputParametersInvalidFormat = 6100,
            ViolateBusinessRules = 6200,
            BankDoesNotSupport = 6203,
            DuplicateTransactionId = 6206,
            ExceedAmountOfTransaction = 6212,
            BankOrCardInformationInvalid = 7200,
            ConnectToBankFail = 7300
        }

        public static ErrorCodeEnum ParseErrorCode(string code)
        {
            try
            {
                return (ErrorCodeEnum) Enum.Parse(typeof(ErrorCodeEnum), code);
            }
            catch
            {
                return ErrorCodeEnum.UnknownErrorCode;
            }
        }

        public static string ErrorCodeDetail(ErrorCodeEnum code)
        {
            switch (code)
            {
                case ErrorCodeEnum.NoError:
                    return "Successfully";
                case ErrorCodeEnum.InvalidChecksum:
                    return "Fake response";
                case ErrorCodeEnum.AuthenticateFailure:
                    return "Authenticate failure";
                case ErrorCodeEnum.BankDoesNotSupport:
                    return "Bank doesn’t support";
                case ErrorCodeEnum.BankOrCardInformationInvalid:
                    return "Bank or card information invalid";
                case ErrorCodeEnum.ConnectToBankFail:
                    return "Connect to bank fail";
                case ErrorCodeEnum.DuplicateTransactionId:
                    return "Duplicate transaction id";
                case ErrorCodeEnum.ExceedAmountOfTransaction:
                    return "Exceed amount of transaction";
                case ErrorCodeEnum.InputParametersInvalidFormat:
                    return "Input parameters invalid format";
                case ErrorCodeEnum.SystemBusy:
                    return "System busy";
                case ErrorCodeEnum.ViolateBusinessRules:
                    return "Violate business rules";
                default:
                    return string.Empty;

            }
        }
    }
}