using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util.Common;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.Transaction;
using WebApplication.Models.User;

namespace WebApplication.Lib.Bll
{
    public class TransactionBll
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const int status_all = -1;
        public static ResultModel GetTransHistory(UserModel currentUser, string beginDate, string endDate,
                                                    int status, string searchType, string searchValue, string email)
        {
            var resultModel = new ResultModel();
            var userModel = new UserModel();
            long searchValueLong = 0;
            long transId = 0;
            string orderCode = "";

            try
            {
                using (var dbContext = new Entities())
                {

                    #region [Validation]
                    if (currentUser == null)
                    {
                        resultModel.setCode(Result.AUTH);
                        return resultModel;
                    }

                    if (!status.Equals(StatusEnum.Active) && !status.Equals(StatusEnum.InActive))
                    {
                        status = status_all;
                    }

                    try
                    {
                        searchValueLong = long.Parse(searchValue);
                    }
                    catch (Exception)
                    {
                        searchValueLong = 0;
                    }

                    switch (searchType)
                    {
                        case "ALL":
                            transId = searchValueLong;
                            orderCode = searchValue;
                            break;

                        case "TID":
                            transId = searchValueLong;
                            break;

                        case "ORDERID":
                            orderCode = searchValue;
                            break;
                        default:
                            transId = searchValueLong;
                            orderCode = searchValue;
                            break;
                    }
                    #endregion

                    #region [ListAllTrans]
                    var accountAdmin = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().Equals(currentUser.Email.Trim())
                                                                && m.RoleId == RoleEnum.Admin);
                    if (accountAdmin != null)
                    {
                        beginDate = beginDate + " 00:00:00";
                        endDate = endDate + " 23:59:59";
                        string createDate_Str = "AND t.CreatedDate between '" + Utils.converseDMYToYMD(beginDate) + "' and '" + Utils.converseDMYToYMD(endDate) + "'";
                        string status_Str = "";
                        string transId_Str = "";
                        string transIdOrOrderId_Str = "";
                        string joinOrder_Str1 = "";
                        string email_Str2 = "";
                        string orderCode_Str = "";

                        joinOrder_Str1 = "LEFT JOIN [Order] o ON o.Id = t.OrderId LEFT JOIN Account a ON o.UserId = a.Id ";
                        if (!status.Equals(status_all))
                        {
                            status_Str = "AND  [Status] = " + status;
                        }
                        if (transId != 0 && orderCode != null && orderCode.Trim() != null)
                        {
                            transIdOrOrderId_Str = "AND t.Id = " + transId + "OR o.Code = '" + orderCode + "'";
                        }
                        else
                            if (transId != 0)
                            {
                                transId_Str = "AND t.Id = " + transId;
                            }
                            else
                                if (orderCode.Trim() != null && !orderCode.Equals(""))
                                {
                                    orderCode_Str = "AND o.Code = '" + orderCode + "'";
                                }

                        if (email != null && email.Trim() != null)
                        {
                            email_Str2 = " AND a.Email = '" + email + "'";
                            createDate_Str = "";
                        }
                        string query = "Select * From TransactionHistory t " + joinOrder_Str1 + " WHERE 1 = 1 " + createDate_Str + status_Str + transId_Str + orderCode_Str + transIdOrOrderId_Str + email_Str2;
                        var trans = dbContext.TransactionHistories.SqlQuery(query).ToList<TransactionHistory>();

                        if (trans != null)
                        {
                            List<TransactionModel> listTrans = new List<TransactionModel>();
                            if (trans != null)
                            {
                                foreach (var tran in trans)
                                {
                                    TransactionModel model = new TransactionModel();
                                    model.Id = tran.Id;
                                    model.OrderId = tran.OrderId;
                                    model.OrderCode = tran.Order.Code;
                                    model.CreatedDate = tran.CreatedDate;
                                    model.Email = tran.Order.Account.Email;
                                    model.ExtraInfo = tran.ExtraInfo;
                                    model.Status = tran.Status;
                                    model.PaymentType = tran.PaymentType;
                                    model.TransferAmount = tran.TransferAmount;
                                    model.TotalPrice = tran.Order.TotalPrice;

                                    listTrans.Add(model);
                                }
                            }

                            resultModel.setCode(Result.SUCCESS);
                            resultModel.Data = listTrans;
                        }
                    }
                    else
                    {
                        resultModel.setCode(Result.ACCOUNT_NOT_ADMIN);
                    }
                    #endregion
                }
            }
            catch (Exception exception)
            {
                Log.Error("Reset Password", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            return resultModel;
        }

        public static ResultModel GetTransHistoryForUser(UserModel currentUser)
        {
            var resultModel = new ResultModel();
            var userModel = new UserModel();

            try
            {
                using (var dbContext = new Entities())
                {

                    #region [Validation]
                    if (currentUser == null)
                    {
                        resultModel.setCode(Result.AUTH);
                        return resultModel;
                    }
                    #endregion

                    #region [ListAllTrans]
                    var account = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().Equals(currentUser.Email.ToLower().Trim()));
                    if (account != null)
                    {
                        var email = currentUser.Email;
                        var email_Str1 = "";
                        var email_Str2 = "";
                        if (email != null && email.Trim() != null)
                        {
                            email_Str1 = "LEFT JOIN [Order] o ON o.Id = t.OrderId LEFT JOIN Account a ON o.UserId = a.Id ";
                            email_Str2 = " AND a.Email = '" + email + "'";
                        }
                        string query = "Select * From TransactionHistory t " + email_Str1 + " WHERE 1=1 " + email_Str2;
                        var trans = dbContext.TransactionHistories.SqlQuery(query).ToList<TransactionHistory>();

                        if (trans != null)
                        {
                            List<TransactionModel> listTrans = new List<TransactionModel>();
                            if (trans != null)
                            {
                                foreach (var tran in trans)
                                {
                                    TransactionModel model = new TransactionModel();
                                    model.Id = tran.Id;
                                    model.OrderId = tran.OrderId;
                                    model.OrderCode = tran.Order.Code;
                                    model.CreatedDate = tran.CreatedDate;
                                    model.ExtraInfo = tran.ExtraInfo;
                                    model.Status = tran.Status;
                                    model.PaymentType = tran.PaymentType;
                                    model.TransferAmount = tran.TransferAmount;
                                    model.TotalPrice = tran.Order.TotalPrice;

                                    listTrans.Add(model);
                                }
                            }

                            resultModel.setCode(Result.SUCCESS);
                            resultModel.Data = listTrans;
                        }
                    }
                    else
                    {
                        resultModel.setCode(Result.AUTH);
                    }
                    #endregion
                }
            }
            catch (Exception exception)
            {
                Log.Error("Reset Password", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            return resultModel;
        }
    }
}