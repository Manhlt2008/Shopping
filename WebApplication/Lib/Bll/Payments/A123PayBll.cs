using System;
using System.Linq;
using System.Reflection;
using log4net;
using Newtonsoft.Json;
using WebApplication.Lib.Dal.ConfigData;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util.Common;
using WebApplication.Lib.Util.Constant;
using WebApplication.Lib.Util.Security;
using WebApplication.Models.APIModel._123pay;
using WebApplication.Models.Order;
using TransactionScope = System.Transactions.TransactionScope;
using System.Configuration;

namespace WebApplication.Lib.Bll.Payments
{
    public static partial class A123PayBll
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static string cancelURL = ConfigurationManager.AppSettings["CANCELURL"];
        private static string redirectURL = ConfigurationManager.AppSettings["REDIRECTURL"];
        private static string errorURL = ConfigurationManager.AppSettings["ERRORURL"];
        /// <summary>
        /// Create Order record on 123Pay
        /// </summary>
        /// <param name="model">CheckoutModel</param>
        /// <param name="order">System Order Record</param>
        /// <param name="userHostAddress">Client IP Address</param>
        public static Create123PayResponseModel CreateOrder(CheckoutModel model, Order order, string userHostAddress)
        {
            try
            {
                var payment123PayConfig = Payment123PayConfig.Instance;

                var createorderRequest = new Create123PayRequestModel
                {
                    AddInfo = string.Empty,
                    BankCode = BankCodeEnum._123PayLocalDebit,
                    Checksum = string.Empty,
                    ClientIp = userHostAddress,

                    CustomerAddress = model.DeliveringInfo.Address,
                    CustomerDob = model.DeliveringInfo.Dob.ToString("dd/MM/yyyy"),
                    CustomerGender = model.DeliveringInfo.Gender.Equals("0") ? "M" : "F",
                    CustomerMail = model.DeliveringInfo.Mail,
                    CustomerName = model.DeliveringInfo.FirstName + " " + model.DeliveringInfo.LastName,
                    CustomerPhone = model.DeliveringInfo.Phone,

                    MerchantCode = payment123PayConfig.MerchantCode,
                    Description = string.Empty,
                    TotalAmount = order.TotalPrice,
                    MTransactionId = string.Format("{0}-{1}-{2}-{3}", order.Code, order.Id, Utils.GenerateCode(), DateTime.Today.ToString("yyyyMMDDHHmmss")),
                    //CancelUrl = Utils.GetUrlPath("A123ErrorOrCancelCallback", "Order"),
                    //ErrorUrl = Utils.GetUrlPath("A123ErrorOrCancelCallback", "Order"),
                    //RedirectUrl = Utils.GetUrlPath("A123CompleteSuccessCallback", "Order"),
                    CancelUrl = cancelURL,
                    ErrorUrl = errorURL,
                    RedirectUrl = redirectURL,
                    Passcode = payment123PayConfig.PassCode
                };

                var prepareChecksum =
                    string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}",
                        createorderRequest.MTransactionId, createorderRequest.MerchantCode, createorderRequest.BankCode,
                        createorderRequest.TotalAmount, createorderRequest.ClientIp, createorderRequest.CustomerName,
                        createorderRequest.CustomerAddress, createorderRequest.CustomerGender,
                        createorderRequest.CustomerDob, createorderRequest.CustomerPhone,
                        createorderRequest.CustomerMail, createorderRequest.CancelUrl, createorderRequest.RedirectUrl,
                        createorderRequest.ErrorUrl, payment123PayConfig.PassCode, payment123PayConfig.SecretKey);

                var checksum = Utils.Sha1(prepareChecksum);

                createorderRequest.Checksum = checksum;

                var responseStr = "{result: " +
                                  InlamiaHttpRequest.InlamiaHttpRequest.Post(createorderRequest.GetDictionary(),
                                      payment123PayConfig.CreateOrderUrl) + " }";

                var createResponseJson = JsonConvert.DeserializeObject<Crate123PayResponseJsonModel>(responseStr, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                var response = createResponseJson.GetResponseModel();

                var resturnCode = ParseErrorCode(response.ReturnCode);

                if (resturnCode != ErrorCodeEnum.NoError)
                {
                    Log.Error("Create Order");
                    Log.Error(responseStr);
                    return null;
                }
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        var transaction123Pay = new Transaction123Pay
                        {
                            BankCode = createorderRequest.BankCode,
                            OrderId = order.Id,
                            CallbackDescription = response.Description,
                            CallbackErrorCode = response.ReturnCode,
                            CallbackTransactionId = response.TransactionId,
                            ClientIP = createorderRequest.ClientIp,
                            CustomerAddress = createorderRequest.CustomerAddress,
                            CustomerDOB = createorderRequest.CustomerDob,
                            CustomerGender = createorderRequest.CustomerGender,
                            CustomerMail = createorderRequest.CustomerMail,
                            CustomerName = createorderRequest.CustomerName,
                            CustomerPhone = createorderRequest.CustomerPhone,
                            Description = createorderRequest.Description,
                            MTransactionId = createorderRequest.MTransactionId,
                            UserId = order.UserId,
                            CreatedDate = DateTime.Now,
                            Status = StatusEnum.Active,
                            TotalAmount = createorderRequest.TotalAmount
                        };

                        dbContext.Transaction123Pay.Add(transaction123Pay);

                        var updateOrder = dbContext.Orders.First(m => m.Id == order.Id);
                        updateOrder.PaymentType = StatusEnum.PaymentMethodEnum.A123Pay.GetHashCode();

                        var history = new TransactionHistory
                        {
                            CreatedDate = DateTime.Now,
                            ExtraInfo = responseStr,
                            OrderId = order.Id,
                            PaymentType = StatusEnum.PaymentMethodEnum.A123Pay.GetHashCode(),
                            TransferAmount = order.TotalPrice,
                            Status = StatusEnum.Active
                        };
                        dbContext.TransactionHistories.Add(history);

                        dbContext.SaveChanges();
                        scope.Complete();
                    }
                }

                return response;
            }
            catch (Exception exception)
            {
                Log.Error("CreateOrder", exception);
                return null;
            }
        }

        public static bool CompleteSuccess(string transactionId, string time, string ticket, string gwsRd)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        var transaction123Pay =
                            dbContext.Transaction123Pay.First(m => m.MTransactionId.Equals(transactionId));

                        transaction123Pay.CompleteTime = time;
                        transaction123Pay.Ticket = ticket;

                        transaction123Pay.Order.Status = OrderStatusEnum.Purchased;

                        dbContext.SaveChanges();

                        scope.Complete();

                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("CompleteSuccess", exception);
            }
            return false;
        }

        public static bool TransactionCancelOrReject(string transactionId, string time, string ticket, string gwsRd)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        var transaction123Pay =
                            dbContext.Transaction123Pay.First(m => m.MTransactionId.Equals(transactionId));

                        transaction123Pay.CompleteTime = time;
                        transaction123Pay.Ticket = ticket;

                        transaction123Pay.Order.Status = OrderStatusEnum.Deleted;

                        dbContext.SaveChanges();

                        scope.Complete();

                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("CompleteSuccess", exception);
            }
            return false;
        }

        public static Create123PayRequestModel GetTransactionById(string transactionId)
        {
            Create123PayRequestModel transactionModel = new Create123PayRequestModel();
            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        var transaction = dbContext.Transaction123Pay.FirstOrDefault(m => m.MTransactionId == transactionId);
                        if (transaction != null)
                        {
                            transactionModel = new Create123PayRequestModel
                            {
                                MTransactionId = transaction.MTransactionId,
                                BankCode = transaction.BankCode,
                                TotalAmount = transaction.TotalAmount,
                                ClientIp = transaction.ClientIP,
                                CustomerName = transaction.CustomerName,
                                CustomerAddress = transaction.CustomerAddress,
                                CustomerGender = transaction.CustomerGender,
                                CustomerDob = transaction.CustomerDOB,
                                CustomerPhone = transaction.CustomerPhone,
                                CustomerMail = transaction.CustomerMail,
                                Description = transaction.Description,
                                ReturnCode = transaction.ReturnCode,
                                Ts = transaction.Ts,
                                ReturnDescription = transaction.ReturnDescription,
                                OpAmount = transaction.OpAmount
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetTransactionById (transactionId : " + transactionId.ToString() + ")" + ex.ToString());
            }
            return transactionModel;
        }

        public static void UpdateTransactionFromNotify(string mtransactionId, string transactionStatus, string description, string ts)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        var tran = dbContext.Transaction123Pay.FirstOrDefault(m => m.MTransactionId == mtransactionId);
                        if (tran != null && tran.Id > 0)
                        {
                                tran.ReturnCode = transactionStatus;
                                tran.ReturnDescription = description;
                                tran.Ts = ts;
                                tran.ModifyDate = DateTime.Now;
                                dbContext.SaveChanges();
                                scope.Complete();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("UpdateTransactionFromNotify (transactionId : " + mtransactionId.ToString() + ")" + ex.ToString());
            }
            
        }
    }
}