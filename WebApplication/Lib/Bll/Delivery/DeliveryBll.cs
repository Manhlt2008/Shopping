using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Helpers;
using log4net;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using WebApplication.Lib.Dal.ConfigData;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.APIModel.Delivery;
using WebApplication.Models.GHTK;
using WebApplication.Models.Order;
using WebApplication.Models.OrderDetail;

namespace WebApplication.Lib.Bll.Delivery
{
    public partial class DeliveryBll
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static Models.GHTK.ResponsePriceCalculator PriceCalculator(Models.GHTK.RequestPriceCalculator requestPriceCalculator)
        {
            try
            {

            }
            catch (Exception exception)
            {
                Log.Error("PriceCalculator", exception);
            }
            return null;
        }
        public static DeliveryTransactionModel CreateDeliveryTransactionBySupplier(double price, List<OrderDetailModel> orderDetailModels, Models.GHTK.AddressCustom pickAddress, Models.GHTK.AddressCustom deiveryAddress)
        {
            try
            {
                using (var dbcontext = new Entities())
                {
                    var orderDetailIds = orderDetailModels.Select(m => m.Id).ToList();
                    var order =
                        dbcontext.OrderDetails.Where(m => m.Id == orderDetailIds.First()).Select(m => m.Order).First();
                    var suppliers =
                        dbcontext.OrderDetails.Where(m => orderDetailIds.Contains(m.Id))
                            .Select(m => m.Product.SupplierProducts.First().Supplier)
                            .GroupBy(m => m)
                            .Select(m => m.Key)
                            .ToList();

                    var list = new List<DeliveryTransactionOrderModel>();
                    foreach (var supplier in suppliers)
                    {
                        var dtods = new List<DeliveryTransactionOrderDetailModel>();
                        var ods =
                            orderDetailModels.Where(
                                m => supplier.SupplierProducts.Select(s => s.ProductId).Contains(m.ProductId)).ToList();

                        foreach (var orderDetailModel in ods)
                        {
                            var d = new DeliveryTransactionOrderDetailModel
                            {
                                OrderDetailId = orderDetailModel.Id,
                                ProductName = orderDetailModel.ProductName,
                                Quantity = orderDetailModel.Quantity,
                            };
                            dtods.Add(d);
                        }

                        list.Add(new DeliveryTransactionOrderModel
                        {
                            DeliveryTransactionOrderDetailModels = dtods,
                            OrderId = order.Id,
                            SupplierId = supplier.Id,
                            CustomerAddress = deiveryAddress.Address,
                            CustomerDistrict = deiveryAddress.District,
                            CustomerProvince = deiveryAddress.Province,
                            SupplierAddress = pickAddress.Address,
                            SupplierName = supplier.Name,
                            SupplierProvince = pickAddress.Province,
                            SupplierDistrict = pickAddress.District,
                            Price = price
                        });
                    }
                    var deliverytransaction = new DeliveryTransactionModel
                    {
                        DeliveryTransactionOrderModels = list,
                        CreatedByAccountId = UserBll.GetUser().Id,
                    };
                    return deliverytransaction;
                }

            }
            catch (Exception exception)
            {
                Log.Error("GroupOrderDetailBySupplier", exception);
                return null;
            }

        }

        public static Models.GHTK.Reponse SendDeliveryTransaction(DeliveryTransactionModel deliveryTransactionModel)
        {
            try
            {
                var deliveryConfig = DeliveryConfig.Instance;
                var listproducts = new List<Models.GHTK.Product>();
                var deliverytransactionmodel0 = deliveryTransactionModel.DeliveryTransactionOrderModels[0];
                var order = new Models.GHTK.Order
                {
                    SystemOrderId = deliverytransactionmodel0.OrderId,
                    Name = deliverytransactionmodel0.CustomerName,
                    Address = deliverytransactionmodel0.CustomerAddress,
                    Province = deliverytransactionmodel0.SupplierProvince,
                    District = deliverytransactionmodel0.CustomerDistrict,
                    Ward = string.Empty,
                    Street = string.Empty,
                    Tel = string.Empty,
                    Email = string.Empty,
                    PickMoney = deliverytransactionmodel0.Price,
                    PickName = deliverytransactionmodel0.SupplierName,
                    PickAddress = deliverytransactionmodel0.SupplierAddress,
                    PickProvince = deliverytransactionmodel0.SupplierProvince,
                    PickDistrict = deliverytransactionmodel0.SupplierDistrict,
                    PickAddressId = 0,
                    PickEmail = string.Empty,
                    PickTel = string.Empty,
                    PickStreet = string.Empty,
                    PickWard = string.Empty,
                    Note = string.Empty,
                    DeliverWorkShift = 0,
                    IsFreeship = 0
                };
                foreach (var deliverytransaction in deliveryTransactionModel.DeliveryTransactionOrderModels)
                {
                    foreach (var deliverytransactionorder in deliverytransaction.DeliveryTransactionOrderDetailModels)
                    {
                        var product = new Models.GHTK.Product
                        {
                            Name = deliverytransactionorder.ProductName,
                            Price = 0,
                            Quantity = deliverytransactionorder.Quantity,
                            Weight = 0,
                            //TODO
                        };
                        listproducts.Add(product);

                    }
                }
                var orders = new Orders
                {
                    Order = order,
                    Products = listproducts,
                };
                var request = new Request
                {
                    Orders = orders,
                    Token = deliveryConfig.DeliveryApiToken
                };
                var responseStr = InlamiaHttpRequest.InlamiaHttpRequest.Post(JsonConvert.SerializeObject(request),
             deliveryConfig.DeliveryCreateOrderPostUrl);

                var response = JsonConvert.DeserializeObject<Reponse>(responseStr);
                if (response.IsSucess)
                {
                    using (var dbcontext = new Entities())
                    {
                        var deliverytransaction = new DeliveryTransaction
                        {
                            IsSuccess = response.IsSucess,
                            Message = response.Message,
                            Request = JsonConvert.SerializeObject(request),
                            Response = responseStr,
                            CreatedByAccountId = deliveryTransactionModel.CreatedByAccountId,
                            ExtraInfo = string.Empty,
                        };
                        dbcontext.DeliveryTransactions.Add(deliverytransaction);
                        dbcontext.SaveChanges();
                        foreach (var deliveryTransactionOrderModel in deliveryTransactionModel.DeliveryTransactionOrderModels)
                        {
                            var deliveryTransactionOrder = new DeliveryTransactionOrder
                            {
                                SupplierId = deliveryTransactionOrderModel.SupplierId,
                                CustomerAddress = deliveryTransactionOrderModel.CustomerAddress,
                                CustomerDistrict = deliveryTransactionOrderModel.CustomerDistrict,
                                CustomerName = deliveryTransactionOrderModel.CustomerName,
                                CustomerProvince = deliveryTransactionOrderModel.CustomerProvince,
                                DeliverySystemId = deliveryTransactionOrderModel.DeliverySystemId,
                                DeliveryTransactionId = deliverytransaction.Id,
                                ExtraInfo = string.Empty,
                                OrderId = deliverytransactionmodel0.OrderId,
                                Price = deliverytransactionmodel0.Price,
                                Request = JsonConvert.SerializeObject(request),
                                Response = JsonConvert.SerializeObject(deliveryTransactionOrderModel),
                                SupplierAddress = deliveryTransactionOrderModel.SupplierAddress,
                                SupplierDistrict = deliveryTransactionOrderModel.SupplierDistrict,
                                SupplierName = deliveryTransactionOrderModel.SupplierName,
                                SupplierProvince = deliveryTransactionOrderModel.SupplierProvince,
                                Status = deliveryTransactionOrderModel.Status
                            };

                            dbcontext.DeliveryTransactionOrders.Add(deliveryTransactionOrder);
                            dbcontext.SaveChanges();

                            var detailsList = new List<DeliveryTransactionOrderDetail>();

                            foreach (var deliveryTransactionOrderDetailModel in deliveryTransactionOrderModel.DeliveryTransactionOrderDetailModels)
                            {
                                foreach (var successOrder in response.SuccessOrders)
                                {
                                    detailsList.Add(new DeliveryTransactionOrderDetail
                                    {
                                        DelviveryTransactionOrderId = deliveryTransactionOrder.Id,
                                        ProductName = deliveryTransactionOrderDetailModel.ProductName,
                                        Quantity = deliveryTransactionOrderDetailModel.Quantity,
                                        Response = JsonConvert.SerializeObject(deliveryTransactionOrderDetailModel),
                                        Status = successOrder.Status,
                                        OrderDetailId = deliveryTransactionOrderDetailModel.OrderDetailId,
                                    });
                                }
                                foreach (var errorOrder in response.ErrorOrders)
                                {
                                    detailsList.Add(new DeliveryTransactionOrderDetail
                                    {
                                        DelviveryTransactionOrderId = deliveryTransactionOrder.Id,
                                        ProductName = deliveryTransactionOrderDetailModel.ProductName,
                                        Quantity = deliveryTransactionOrderDetailModel.Quantity,
                                        Response = JsonConvert.SerializeObject(deliveryTransactionOrderDetailModel),
                                        OrderDetailId = deliveryTransactionOrderDetailModel.OrderDetailId,
                                    });
                                }


                            }

                            dbcontext.DeliveryTransactionOrderDetails.AddRange(detailsList);
                            dbcontext.SaveChanges();
                        }
                    }


                }
                return response;
            }
            catch (Exception exception)
            {

                Log.Error("SendDeliveryTransaction", exception);
                return null;
            }
        }
        public static bool CreateDeliveryOrder(Orders orders)
        {
            try
            {
                var deliveryConfig = DeliveryConfig.Instance;
                var request = new Request
                {
                    Token = deliveryConfig.DeliveryApiToken,
                    Orders = orders
                };

                var responseStr = InlamiaHttpRequest.InlamiaHttpRequest.Post(JsonConvert.SerializeObject(request),
                    deliveryConfig.DeliveryCreateOrderPostUrl);

                var response = JsonConvert.DeserializeObject<Reponse>(responseStr);
                var resturnCode = response.IsSucess;
                if (resturnCode != true)
                {
                    Log.Error("CreateDeliveryOrder");
                    Log.Error(responseStr);
                    return false;
                }
                using (var dbcontext = new Entities())
                {
                    var deliverytracsaction = new DeliveryTransaction
                    {
                        IsSuccess = response.IsSucess,
                        Response = responseStr,
                        Message = response.Message,
                        ExtraInfo = string.Empty,
                        Request = JsonConvert.SerializeObject(request)
                    };

                    foreach (var item in response.SuccessOrders)
                    {
                        try
                        {
                            var order = dbcontext.Orders.First(m => m.Id == item.OrderId);
                            var orderdetail = dbcontext.OrderDetails.First(m => m.Id == item.OrderId);
                            var supplierproduct =
                                dbcontext.SupplierProducts.First(m => m.ProductId == orderdetail.ProductId);
                            var supplier = dbcontext.Suppliers.First(m => m.Id == supplierproduct.SupplierId);
                            var account = dbcontext.Accounts.First(m => m.Id == order.Account.Id);
                            var deliverytransactionorder = new DeliveryTransactionOrder
                            {
                                DeliveryTransactionId = deliverytracsaction.Id,
                                SupplierId = supplier.Id,
                                CustomerName = account.Firstname + " " + account.Lastname,

                            };
                        }
                        catch (Exception exception)
                        {
                            Log.Error("CreateDeliveryOrder", exception);
                        }

                    }



                    dbcontext.DeliveryTransactions.Add(deliverytracsaction);
                    dbcontext.SaveChanges();
                }

            }
            catch (Exception exception)
            {
                Log.Error("CreateDeliveryOrder", exception);
                return false;
            }
            return false;
        }

        public static ResponseOrdersStatus SendTrackingOrder(long orderid, int type)
        {
            try
            {
                using (var dbcontext = new Entities())
                {
                    var deliveryTransactionOrderDetails =
                        dbcontext.DeliveryTransactionOrders.Where(m => m.OrderId == orderid).ToList();
                    var listdeliveryorderids = new List<long>();
                    foreach (var deliveryTransactionOrderDetail in deliveryTransactionOrderDetails)
                    {
                        listdeliveryorderids.Add(deliveryTransactionOrderDetail.Id);
                    }
                    var deliveryConfig = DeliveryConfig.Instance;
                    var request = new Models.GHTK.CheckOrdersStatus
                    {
                        Token = deliveryConfig.DeliveryApiToken,
                        Ids = listdeliveryorderids,
                        Type = 0
                    };
                    var responseString = InlamiaHttpRequest.InlamiaHttpRequest.Post(JsonConvert.SerializeObject(request),
                        deliveryConfig.DeliveryCheckStatusOrderPostUrlTest);
                    var response = JsonConvert.DeserializeObject<ResponseOrdersStatus>(responseString);
                    var status = response.Success;
                    if (status != false)
                    {
                        foreach (var responseOrdersStatus in response.Orders)
                        {
                            foreach (var deliveryTransactionOrderDetail in deliveryTransactionOrderDetails)
                            {
                                var deliverytransactiondetail =
                                    dbcontext.DeliveryTransactionOrders.First(
                                        m =>
                                            m.Id == deliveryTransactionOrderDetail.Id &&
                                            m.Id == responseOrdersStatus.SystemId);
                                deliverytransactiondetail.Status = responseOrdersStatus.Status;
                                var orderdetail = dbcontext.OrderDetails.First(m => m.OrderId == responseOrdersStatus.SystemId);
                                orderdetail.Status = responseOrdersStatus.Status;
                                dbcontext.SaveChanges();
                            }
                        }
                    }
                    return response;
                }

            }
            catch (Exception exception)
            {
                Log.Error("SendTrackingOrder", exception);
                return null;
            }
        }

        public static bool CancelDeliveryTransaction(long orderid)
        {
            try
            {
                using (var dbcontext = new Entities())
                {
                    var deliveryConfig = DeliveryConfig.Instance;
                    var deliverytransactionorders = dbcontext.DeliveryTransactionOrders.Where(m => m.OrderId == orderid).ToList();
                    var ids = new List<long>();
                    foreach (var deliverytransactionorder in deliverytransactionorders)
                    {
                        ids.Add(deliverytransactionorder.Id);
                    }
                    var cancelOrders = new CancelOrders
                    {
                        Token = deliveryConfig.DeliveryApiToken,
                        SystemIds = ids
                    };
                    var responseString = InlamiaHttpRequest.InlamiaHttpRequest.Post(JsonConvert.SerializeObject(cancelOrders),
                       deliveryConfig.DeliveryCancelOrderPostUrlTest);
                    var response = JsonConvert.DeserializeObject<ResponeCancelOrder>(responseString);
                    if (response != null)
                    {
                        var isCancel = OrderBll.UpdateStatus(orderid, StatusEnum.InActive);
                        if (isCancel)
                        {
                            foreach (var updated in response.UpdatedOrders)
                            {
                                var deliverytransactionorder = deliverytransactionorders.First(m => m.Id == updated);
                                deliverytransactionorder.Status = StatusEnum.InActive;
                                dbcontext.SaveChanges();
                            }
                        }
                    }
                    return true;
                }
            }
            catch (Exception exception)
            {
                Log.Error("CancelDeliveryTransaction", exception);
            }
            return false;
        }
    }
}