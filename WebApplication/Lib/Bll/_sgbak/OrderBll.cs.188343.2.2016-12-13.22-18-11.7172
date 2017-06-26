using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Transactions;
using log4net;
using WebApplication.Lib.Bll.Delivery;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util;
using WebApplication.Lib.Util.Common;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.Order;
using WebApplication.Models.User;
using WebApplication.Models.OrderDetail;

namespace WebApplication.Lib.Bll
{
    public static class OrderBll
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static Order Checkout(CheckoutModel model, UserModel userModel, int paymentType, string emailTemplatePath)
        {
            var checkoutModels = model.Items;
            if (checkoutModels == null || checkoutModels.Count == 0)
            {
                return null;
            }

            if (userModel == null || userModel.Status == StatusEnum.InActive)
            {
                return null;
            }

            var productIds = checkoutModels.Select(m => m.Id).GroupBy(m => m).Select(m => m.Key).ToList();
            var products = ProductBll.FindAllProductByIdIn(productIds);
            if (products.Count != productIds.Count)
            {
                Log.Error("Checkout(): There are some invalid product in cart.\n");

                var productIdsStr = productIds.Aggregate(string.Empty, (current, productId) => current + (productId + ","));

                Log.Info("Product ids: " + productIdsStr);

                return null;
            }

            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        products =
                            dbContext.Products.Where(p => productIds.Contains(p.Id) && p.Status == StatusEnum.Active)
                                .ToList();

                        var orderDetails = new List<OrderDetail>();

                        var district = dbContext.Districts.First(m => m.DistrictId == model.DeliveringInfo.District);
                        var province = dbContext.Provinces.First(m => m.ProvinceId == model.DeliveringInfo.Province);

                        var order = new Order
                        {
                            CreatedDate = DateTime.Now,
                            Status = OrderStatusEnum.New,
                            UserId = userModel.Id,
                            Code = Utils.GenerateCode(),
                            Address = model.DeliveringInfo.Address,
                            District = district.Name,
                            Province = province.Name,
                            PaymentType = paymentType
                        };

                        dbContext.Orders.Add(order);
                        dbContext.SaveChanges();

                        double totalPrice = 0;

                        foreach (var checkoutModel in checkoutModels)
                        {
                            var product = products.FirstOrDefault(m => m.Id == checkoutModel.Id);
                            if (product == null)
                            {
                                Log.Error("Checkout(): product with id " + checkoutModel.Id + " not found.");
                                return null;
                            }

                            product.Quantity -= checkoutModel.Quantity;

                            var orderDetail = new OrderDetail
                            {
                                Discount = 0L,
                                OriginUnitPrice = product.Price,
                                ProductId = product.Id,
                                Quantity = checkoutModel.Quantity,
                                OrderId = order.Id
                            };

                            orderDetails.Add(orderDetail);

                            totalPrice += checkoutModel.Quantity * product.Price;
                        }

                        order.TotalPrice = totalPrice * 1.1;

                        dbContext.OrderDetails.AddRange(orderDetails);

                        dbContext.SaveChanges();

                        scope.Complete();

                        try
                        {

                            var orderDetailTrs = string.Empty;
                            foreach (var orderDetail in orderDetails)
                            {
                                orderDetailTrs +=
                                    string.Format(
                                        "<tr style=\"font - family: 'Helvetica Neue', Helvetica, Arial, sans - serif; box - sizing: border - box; font - size: 14px; margin: 0; \">" +
                                        "    <td class=\"alignright\" style=\"font - family: 'Helvetica Neue', Helvetica, Arial, sans - serif; box - sizing: border - box; font - size: 14px; vertical - align: top; text - align: left; border - top - width: 1px; border - top - color: #eee; border-top-style: solid; margin: 0; padding: 5px 0;\" align=\"right\" width=\"30%\">{0}</td>" +
                                        "    <td class=\"alignright\" style=\"font - family: 'Helvetica Neue', Helvetica, Arial, sans - serif; box - sizing: border - box; font - size: 14px; vertical - align: top; text - align: left; border - top - width: 1px; border - top - color: #eee; border-top-style: solid; margin: 0; padding: 5px 0;\" align=\"right\" width=\"25%\">{1} VND</td>" +
                                        "    <td  class=\"alignright\" style=\"font - family: 'Helvetica Neue', Helvetica, Arial, sans - serif; box - sizing: border - box; font - size: 14px; vertical - align: top; text - align: left; border - top - width: 1px; border - top - color: #eee; border-top-style: solid; margin: 0; padding: 5px 0;\" align=\"right\" width=\"25%\">{2}</td>" +
                                        "    <td  class=\"alignright\" style=\"font - family: 'Helvetica Neue', Helvetica, Arial, sans - serif; box - sizing: border - box; font - size: 14px; vertical - align: top; text - align: left; border - top - width: 1px; border - top - color: #eee; border-top-style: solid; margin: 0; padding: 5px 0;\" align=\"right\" width=\"30%\">{3} VND</td>" +
                                        "</tr>", orderDetail.Product.Name,
                                        orderDetail.OriginUnitPrice, orderDetail.Quantity,
                                        orderDetail.OriginUnitPrice*orderDetail.Quantity);
                            }

                            var body = string.Empty;
                            using (var reader = new StreamReader(emailTemplatePath))
                            {
                                body = reader.ReadToEnd();
                            }
                            body = body.Replace("{0}", string.Format("{0} {1}", userModel.Firstname, userModel.Lastname));
                            body = body.Replace("{1}", order.Code);
                            body = body.Replace("{2}", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                            body = body.Replace("{3}", orderDetailTrs);

                            string check = SendMail.SendEmail(Email.CompanyEmail, Email.CompanyPasswordEmail,
                                userModel.Email, "Xác nhận đơn hàng mới", body);
                        }
                        catch (Exception e)
                        {
                            Log.Error("Send email when creating order", e);
                        }

                        return order;
                    }

                }
            }
            catch (Exception exception)
            {
                Log.Error("Checkout()", exception);
                return null;
            }
        }

        public static Order FindOneByOrderCode(string code, long userId, long productId)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    return
                        dbContext.Orders.Where(
                            m =>
                                m.Code.ToLower().Equals(code.ToLower().Trim()) && m.Status == OrderStatusEnum.Completed &&
                                m.UserId == userId &&
                                m.OrderDetails.FirstOrDefault(d => d.ProductId == productId) != null)
                            .Include(m => m.OrderDetails)
                            .FirstOrDefault();
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindOneByOrderCode()", exception);
            }
            return null;
        }

        public static ResultModel FindOneOrderDetailsByOrderId(UserModel user, long orderId)
        {
            var resultModel = new ResultModel();
            try
            {
                using (var dbContext = new Entities())
                {
                    #region [Validation]
                    if (user == null)
                    {
                        resultModel.setCode(Result.AUTH);
                        return resultModel;
                    }

                    #endregion

                    List<OrderDetailModel> listOrderDetails = new List<OrderDetailModel>();

                    var result = Enumerable.Empty<OrderDetail>();

                    if (user.RoleId.Equals((int)RoleEnum.User))
                    {
                        result = dbContext.OrderDetails.Where(m => m.OrderId.Equals(orderId) && m.Order.UserId.Equals(user.Id)).ToList<OrderDetail>();
                    }
                    else
                    {
                        result = dbContext.OrderDetails.Where(m => m.OrderId.Equals(orderId)).ToList<OrderDetail>();
                    }
                  
                    if (result != null && result.Count() != 0)
                    {
                        foreach (var item in result)
                        {
                            OrderDetailModel orderDetail = new OrderDetailModel();

                            orderDetail.Id = item.Id;
                            orderDetail.ProductId = item.ProductId;
                            orderDetail.OrderId = item.OrderId;
                            orderDetail.OriginUnitPrice = item.OriginUnitPrice;
                            orderDetail.Quantity = item.Quantity;
                            orderDetail.Discount = item.Discount;
                            orderDetail.ProductName = item.Product.Name;
                            orderDetail.CategoryName = item.Product.Category.Name;

                            listOrderDetails.Add(orderDetail);
                        }
                    }
                    resultModel.setCode(Result.SUCCESS);
                    resultModel.Data = listOrderDetails;
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindOneOrderDetailsByOrderId()", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            return resultModel;
        }

        internal static List<Order> FindAllByUserNameOrOrderCode(string queryValue, List<int> orderBySettings = null)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var query = "SELECT [Order].* FROM [Order], [Account] WHERE [Order].UserId = [Account].Id ";

                    if (queryValue != null && !queryValue.Trim().Equals(string.Empty))
                    {
                        query +=
                            string.Format(
                                " AND ([Code] LIKE N'%{0}%' OR [Account].Email LIKE N'%{0}%' OR ([Account].Firstname + ' ' + [Account].Lastname) LIKE N'%{0}%') ",
                                StringUtil.RemoveVietnameseTone(
                                    StringUtil.RemoveSign4VietnameseString(queryValue.ToLower().Trim())));
                    }

                    var isBeginOrderQuery = false;

                    if (orderBySettings != null)
                    {
                        if (!orderBySettings.Contains(StatusEnum.OrderEnum.OrderBySetting.All))
                        {
                            foreach (var orderBySetting in orderBySettings)
                            {
                                var status = 0;

                                switch (orderBySetting)
                                {
                                    case StatusEnum.OrderEnum.OrderBySetting.Completed:
                                        status = OrderStatusEnum.Completed;
                                        break;
                                    case StatusEnum.OrderEnum.OrderBySetting.New:
                                        status = OrderStatusEnum.New;
                                        break;
                                    case StatusEnum.OrderEnum.OrderBySetting.Delevering:
                                        status = OrderStatusEnum.Delevering;
                                        break;
                                    case StatusEnum.OrderEnum.OrderBySetting.Purchased:
                                        status = OrderStatusEnum.Purchased;
                                        break;
                                    case StatusEnum.OrderEnum.OrderBySetting.Reject:
                                        status = OrderStatusEnum.Reject;
                                        break;
                                }

                                if (!isBeginOrderQuery)
                                {
                                    isBeginOrderQuery = true;
                                    query += string.Format("AND ([Order].[Status] = {0} ", status);
                                }
                                else
                                {
                                    query += string.Format("OR [Order].[Status] = {0} ", status);
                                }

                            }

                            if (isBeginOrderQuery)
                            {
                                query += ") ";
                            }

                            if (orderBySettings.Contains(StatusEnum.OrderEnum.OrderBySetting.Default) || orderBySettings.Count == 0)
                            {
                                query += "ORDER BY [Order].CreatedDate ";
                            }

                            if (orderBySettings.Contains(StatusEnum.OrderEnum.OrderBySetting.OrderCode))
                            {
                                query += "ORDER BY [Order].Code ";
                            }
                        }
                    }

                    var orders = dbContext.Orders.SqlQuery(query)
                            .AsQueryable()
                            .Include(m => m.Account)
                            .Include(m => m.OrderDetails)
                            .Include("OrderDetails.Product")
                            .ToList();

                    foreach (var order in orders)
                    {
                        order.Account = order.Account;
                        order.OrderDetails = order.OrderDetails;
                    }

                    return orders;
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindAllByUserNameOrOrderCode", exception);
            }

            return Enumerable.Empty<Order>().ToList();
        }

        public static Order FindOneById(long orderId)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    return
                        dbContext.Orders.Where(m => m.Id == orderId)
                            .Include(m => m.OrderDetails)
                            .Include("OrderDetails.Product")
                            .Include("OrderDetails.Product.Category")
                            .Include(m => m.Account)
                            .FirstOrDefault();
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindOneById()", exception);
            }
            return null;
        }
        public static OrderDetail FindOneOrderdetailIdByOrderDetailId(long orderdetailId)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    return
                        dbContext.OrderDetails.Where(m => m.Id == orderdetailId)
                            .Include(m => m.Product)
                            .Include(m => m.Product.SupplierProducts)
                            .Include(m => m.Order)
                            .Include("Product.SupplierProducts.Product")
                            .Include("Product.SupplierProducts.Supplier")
                            .FirstOrDefault();
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindOneByOrderDetailsID", exception);
            }
            return null;
        }

        public static ResultModel FindOrderByUserId(UserModel currentUser, long userId)
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
                    if (account.RoleId.Equals((int)RoleEnum.User))
                    {
                        if (!account.Id.Equals(userId))
                        {
                            resultModel.setCode(Result.INVALID_DATA);
                            return resultModel;
                        }
                    }
                    if (account != null)
                    {
                        var orders = dbContext.Orders.Where(m => m.UserId.Equals(userId)).ToList<Order>().OrderByDescending(m => m.CreatedDate);
                        List<OrderModel> ordersModel = new List<OrderModel>();
                        if (orders != null && orders.Count() != 0)
                        {
                            foreach (var item in orders)
                            {
                                OrderModel model = new OrderModel();

                                model.Id = item.Id;
                                model.UserId = item.UserId;
                                model.CreatedDate = item.CreatedDate;
                                model.CreatedDateStr = item.CreatedDate.ToString();
                                model.TotalPrice = item.TotalPrice;
                                model.Status = item.Status;
                                model.Code = item.Code;

                                ordersModel.Add(model);
                            }
                        }

                        resultModel.setCode(Result.SUCCESS);
                        resultModel.Data = ordersModel;
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

        public static bool UpdateStatus(long orderId, int newStatus)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var order = dbContext.Orders.FirstOrDefault(m => m.Id == orderId);
                    if (order != null)
                    {
                        if (StatusEnum.InActive==newStatus)
                        {
                            var isCancel = DeliveryBll.CancelDeliveryTransaction(orderId);
                        }                     
                        order.Status = newStatus;
                        dbContext.SaveChanges();
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("UpdateStatus", exception);
            }

            return false;
        }
        public static bool IncreaseSendMailCounter(long orderdetailid)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var orderDetail = dbContext.OrderDetails.FirstOrDefault(m => m.Id == orderdetailid);
                    if (orderDetail != null)
                    {
                        orderDetail.SendmailCounter++;
                        dbContext.SaveChanges();
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("IncreaseSendMailCounter", exception);
            }

            return false;
        }

        public static bool UpdateOrder(UpdateOrderModelRequest request)
        {
            var orderId = long.MinValue;
            var status = StatusEnum.Active;

            if (request == null || request.OrderDetails.Count == 0)
            {
                return false;
            }

            var orderDetails = request.OrderDetails;

            orderId = request.OrderId;
            status = request.Status;

            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        double totalPrice = 0;

                        var order = dbContext.Orders.FirstOrDefault(m => m.Id == orderId);

                        if (order == null)
                        {
                            Log.Info(string.Format("Order {0} not found", orderId));
                            return false;
                        }

                        var productIds = orderDetails.Select(m => m.ProductId).ToList();

                        var productSize = dbContext.Products.Count(m => productIds.Contains(m.Id));
                        if (productSize != productIds.Count)
                        {
                            Log.Info("Having some invalid product.");
                            return false;
                        }

                        var orderDetailEntities = dbContext.OrderDetails.Where(m => m.OrderId == orderId).ToList();
                        foreach (var orderDetailEntity in orderDetailEntities)
                        {
                            orderDetailEntity.Status = StatusEnum.InActive;
                        }

                        foreach (var updateOrderDetailModelRequest in orderDetails)
                        {
                            if (updateOrderDetailModelRequest.OrderDetailId == -1)
                            {
                                var orderDetail = new OrderDetail
                                {
                                    Discount = updateOrderDetailModelRequest.Discount,
                                    ProductId = updateOrderDetailModelRequest.ProductId,
                                    OrderId = orderId,
                                    OriginUnitPrice = updateOrderDetailModelRequest.Price,
                                    Quantity = updateOrderDetailModelRequest.Quantity,
                                    Status = StatusEnum.Active
                                };

                                dbContext.OrderDetails.Add(orderDetail);
                            }
                            else
                            {
                                var orderDetail = orderDetailEntities.FirstOrDefault(m => m.Id == updateOrderDetailModelRequest.OrderDetailId);
                                if (orderDetail == null)
                                {
                                    orderDetail =
                                        dbContext.OrderDetails.FirstOrDefault(
                                            m => m.Id == updateOrderDetailModelRequest.OrderDetailId);

                                    if (orderDetail == null)
                                    {
                                        Log.Info(string.Format("Order Detail [{0}] not found.",
                                            updateOrderDetailModelRequest.OrderDetailId));
                                        return false;
                                    }
                                }

                                orderDetail.OriginUnitPrice = updateOrderDetailModelRequest.Price;
                                orderDetail.Quantity = updateOrderDetailModelRequest.Quantity;
                                orderDetail.Discount = updateOrderDetailModelRequest.Discount;
                                orderDetail.Status = StatusEnum.Active;
                            }

                            totalPrice += updateOrderDetailModelRequest.Quantity * updateOrderDetailModelRequest.Price;
                        }

                        order.TotalPrice = totalPrice;
                        order.Status = status;

                        dbContext.SaveChanges();

                        scope.Complete();

                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("UpdateOrder()", exception);
            }
            return false;
        }

        public static object CreateOrder(UpdateOrderModelRequest request)
        {
            object error = new { IsSuccess = false, Id = 0, Code = "ERROR" };

            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        var user = dbContext.Accounts.FirstOrDefault(m => m.Id == request.UserId);
                        if (user == null)
                        {
                            Log.Info(string.Format("User with ID {0} not found.", request.UserId));
                            return error;
                        }

                        var productIds = request.OrderDetails.Select(m => m.ProductId).ToList();
                        var products = dbContext.Products.Where(m => productIds.Contains(m.Id)).ToList();

                        if (products.Count != productIds.Count)
                        {
                            Log.Info("Contain product(s) that not existed in db.");
                            Log.Info(productIds);
                            return error;
                        }

                        var order = new Order
                        {
                            UserId = user.Id,
                            CreatedDate = DateTime.Now,
                            Status = request.Status,
                            Code = Utils.GenerateCode()
                        };

                        dbContext.Orders.Add(order);

                        dbContext.SaveChanges();

                        double totalPrice = 0;
                        var orderDetails = new List<OrderDetail>();

                        foreach (var detail in request.OrderDetails)
                        {
                            var product = products.FirstOrDefault(m => m.Id == detail.ProductId);

                            if (product == null)
                            {
                                Log.Error("Checkout(): product with id " + detail.ProductId + " not found.");
                                return error;
                            }

                            product.Quantity -= detail.Quantity;

                            var orderDetail = new OrderDetail
                            {
                                Discount = 0L,
                                OriginUnitPrice = product.Price,
                                ProductId = product.Id,
                                Quantity = detail.Quantity,
                                OrderId = order.Id
                            };

                            orderDetails.Add(orderDetail);

                            totalPrice += detail.Quantity * product.Price;
                        }

                        order.TotalPrice = totalPrice;

                        dbContext.OrderDetails.AddRange(orderDetails);

                        dbContext.SaveChanges();

                        scope.Complete();

                        return new { IsSuccess = true, Id = order.Id, Code = order.Code };

                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("CreateOrder()", exception);
            }

            return error;
        }

        public static bool IsAllowCreateOrder()
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var orderSettings = SettingsBll.GetSettings(SettingsBll.SettingNames.OrderLimitedOfEachDay);

                    var number = -1;
                    int.TryParse(orderSettings.First(m => m.Key.Equals(SettingsBll.SettingNames.OrderLimitedOfEachDay)).Value, out number);

                    if (number == -1)
                    {
                        return true;
                    }

                    var todayOrder = dbContext.Orders.Count(m => EntityFunctions.TruncateTime(m.CreatedDate) == DateTime.Today && m.Status != StatusEnum.InActive);

                    return todayOrder <= number;
                }
            }
            catch (Exception exception)
            {
                Log.Error("IsAllowCreateOrder", exception);
            }

            return false;
        }
        public static List<OrderDetail> GetOrderDetailByOrderId(long orderId)
        {
            List<OrderDetail> lstOrder = new List<OrderDetail>();
            try
            {
                using (var dbContext = new Entities())
                {
                    var lstProductId = (from n in dbContext.OrderDetails
                                        where n.OrderId == orderId
                                        select n.Id).ToList();

                    lstOrder = dbContext.OrderDetails.Where(m => lstProductId.Contains(m.Id)).ToList();
                }
            }
            catch (Exception exception)
            {
                Log.Error("GetOrderDetailByOrderId", exception);
            }

            return lstOrder;
        }
    }
}
