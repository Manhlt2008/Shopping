using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Models.Order;
using WebApplication.Models.Product;
using WebApplication.Models.Review;
using WebApplication.Filters;
using WebApplication.Lib.Bll.Delivery;
using WebApplication.Lib.Bll.Payments;
using WebApplication.Lib.Util.Common;
using WebApplication.Lib.Util.Constant;
using WebApplication.Lib.Util.Theme;
using WebApplication.Models.APIModel.Delivery;
using WebApplication.Models.Category;
using WebApplication.Models.OrderDetail;
using WebApplication.Lib.Bll.InlamiaHttpRequest;
using WebApplication.Lib.Dal.ConfigData;
using WebApplication.Models.GHTK;
using WebApplication.Lib.Bll.ApiHelper.DealToDay;
using System.Web.Configuration;

namespace WebApplication.Controllers
{
    public class OrderController : Controller
    {
        private void FindAllOrder(long productId, string sort = "default", int order = 1, int limit = 6, int page = 1)
        {
            var products = ProductBll.FindAllProductHavingReview();
            var product = ProductBll.FindById(productId);
            int totalReview;
            var reviews = ReviewBll.FindByProductId(productId, sort, order, limit, page, out totalReview);

            if (productId != 0 && product == null)
            {
                throw new HttpException(500, "Product Not Found.");
            }

            if (productId == 0)
            {
                product = new Lib.Dal.DbContext.Product
                {
                    Id = 0,
                    Name = "All Products"
                };
            }

            ViewBag.Product = new ProductViewModel(product);
            ViewBag.Products = products.Select(p => new ProductViewModel(p)).ToList();
            ViewBag.Reviews = reviews.Select(r => new ReviewViewModel(r)).ToList();
            ViewBag.DisplayStyle = "grid";
            ViewBag.Sort = sort;
            ViewBag.Order = order;
            ViewBag.TotalReview = totalReview;
            ViewBag.Page = page;
            ViewBag.Limit = limit;
        }

        //
        // GET: /Cart/

        public ActionResult Index()
        {
            return RedirectToAction("Cart");

        }

        [HttpPost]
        public JsonResult GetDistrictsByProvince(string provinceId)
        {
            var districts = AddressBll.GetDistrictsByProvince(provinceId);

            var districtObjs = new List<object>();

            foreach (var district in districts)
            {
                districtObjs.Add(new
                {
                    Id = district.DistrictId,
                    Name = district.Name
                });
            }

            return Json(districtObjs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Cart()
        {
            ViewBag.IsAllowCreateOrder = OrderBll.IsAllowCreateOrder();
            return View(ThemeName.GetView(ThemeName.ViewName.OrderViewNameEnum.Cart));
        }

        [AuthorizeActionFilter]
        public ActionResult Checkout()
        {
            return RedirectToAction("Cart");
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        public ActionResult Manage()
        {
            return View();
        }

        [AuthorizeActionFilter]
        [HttpPost]
        public JsonResult Checkout(CheckoutModel model)
        {
            var user = UserBll.GetUser();
            if (user == null)
            {
                return Json(new { Status = 0, Data = "" });
            }

            var order = OrderBll.Checkout(model, user, model.PaymentMethod, Server.MapPath("~/Common/EmailSendOrderToCustomer.html"));
            var orderObj = new { id = 0L, code = "ERROR" };

            if (order != null)
            {
                orderObj = new { id = order.Id, code = order.Code };
            }

            var listDealToDayOrder = new List<OrderDetail>();

            foreach (var item in order.OrderDetails)
            {
                if (ProductBll.IsDealToDayProduct(item.ProductId))
                {
                    listDealToDayOrder.Add(item);
                }
            }
            if (listDealToDayOrder.Count > 0)
            {
                order.OrderDetails = listDealToDayOrder;
                var responseDealToDay = DealToDayBll.CreateOrder(order);
                if (responseDealToDay == null)
                {
                    orderObj = new { id = 0L, code = "ERROR" };
                    return Json(new { Status = 1, Data = orderObj });
                }
            }
            switch (model.PaymentMethod)
            {
                case 1:
                    return Json(new { Status = 1, Data = orderObj });
                default:
                    var responseModel = A123PayBll.CreateOrder(model, order, Utils.GetIpAddress());
                    if (responseModel == null)
                    {
                        if (order != null) OrderBll.UpdateStatus(order.Id, StatusEnum.InActive);
                        orderObj = new { id = 0L, code = "ERROR" };
                        return Json(new { Status = 1, Data = orderObj });
                    }
                    return Json(new { Status = 1, Data = orderObj, Payment = JsonConvert.SerializeObject(responseModel) });
            }
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        [HttpPost]
        public JsonResult FindAllByUserNameOrOrderCode(string queryString, List<int> filterOptions)
        {
            var orderList = OrderBll.FindAllByUserNameOrOrderCode(queryString, filterOptions);

            var response = new OrderOverviewInfoResponseModelList(orderList);

            return Json(JsonConvert.SerializeObject(response));
        }

        [AuthorizeActionFilter]
        [HttpPost]
        public JsonResult Detail(long orderId)
        {
            var order = OrderBll.FindOneById(orderId);

            var responsee = new OrderOverviewInfoResponseModel(order, true);

            return Json(JsonConvert.SerializeObject(responsee));
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        [HttpPost]
        public JsonResult UpdateStatus(long orderId, int newStatus)
        {
            var isSuccess = OrderBll.UpdateStatus(orderId, newStatus);

            return Json(new { IsSuccess = isSuccess });
        }

        public ActionResult Update(long id)
        {
            var status = DeliveryBll.SendTrackingOrder(id, 0);
            if (status == null)
            {
                ViewBag.StatusTrackingOrder = "Đang chờ xử lý";
            }
            ViewBag.SendMailStatus = TempData["IsSuccess"];
            ViewBag.SendOrderDeliveryIsSuccess = TempData["SendOrderDeliveryIsSuccess"];
            ViewBag.DeliveryTransactionModel = TempData["DeliveryTransactionModel"];
            ViewBag.SendOrderMessage = TempData["SendOrderMessage"];
            ViewBag.Order = OrderBll.FindOneById(id);

            return View("Save");
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        [HttpPost]
        public JsonResult Save(UpdateOrderModelRequest request)
        {
            var status = request.OrderId != -1 ? OrderBll.UpdateOrder(request) : OrderBll.CreateOrder(request);

            return Json(request.OrderId != -1 ? new { IsSuccess = status } : status);
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        public ActionResult Create()
        {
            return View("Save");
        }

        [AuthorizeActionFilter]
        public ActionResult List()
        {
            var categories = CategoryBll.FindAllCategories();
            ViewBag.Categories = categories.Select(cat => new CategoryManageList(cat)).ToList();

            var user = UserBll.GetUser();
            var resultModel = OrderBll.FindOrderByUserId(user, user.Id);
            if (user != null)
            {
                ViewBag.User = user;
                ViewBag.ListOrders = resultModel;
            }
            else
            {
                ViewBag.User = null;
                ViewBag.ListOrders = null;
            }

            return View();
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        public ActionResult SendNewOrderNotifyMail(long id, long orderid)
        {
            var orderdetail = OrderBll.FindOneOrderdetailIdByOrderDetailId(id);
            var supplier = SupplierBll.FindOneByProductId(orderdetail.ProductId);
            //string path = HttpContext.Server.MapPath("~/Common/EmailSendOrderToSupplier.html");
            //var body = string.Format(System.IO.File.ReadAllText(path),
            //    supplier.Supplier.Name,
            //    orderdetail.OrderId,
            //    supplier.Product.Name,
            //    orderdetail.OriginUnitPrice,
            //    orderdetail.Quantity,
            //    orderdetail.OriginUnitPrice * orderdetail.Quantity
            //    );
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Common/EmailSendOrderToSupplier.html")))
            {
                body = reader.ReadToEnd();
            }
            if (supplier == null)
            {
                supplier = new Supplier
                {
                    Name = "Phu Nu Mart"
                };
            }
            body = body.Replace("{0}", supplier.Name);
            body = body.Replace("{1}", orderdetail.Order.Code);
            body = body.Replace("{2}", orderdetail.Product.Name);
            body = body.Replace("{3}", orderdetail.OriginUnitPrice.ToString());
            body = body.Replace("{4}", orderdetail.Quantity.ToString());
            body = body.Replace("{5}", (orderdetail.OriginUnitPrice * orderdetail.Quantity).ToString());
            body = body.Replace("{6}", DateTime.Now.ToString());
            string check = SendMail.SendEmail(Email.CompanyEmail, Email.CompanyPasswordEmail, supplier.Email, "Yêu cầu xác nhận đơn hàng mới", body);
            if (check.Equals("Message Sent Successfully..!!"))
            {
                var isSuccess = OrderBll.IncreaseSendMailCounter(orderdetail.Id);
                TempData["IsSuccess"] = isSuccess;
            }
            return RedirectToAction("Update", new { id = orderid });
        }

        [HttpGet]
        public ActionResult A123CompleteSuccessCallback(string transactionID, string time, string status, string ticket, string gws_rd)
        {
            if (status.Equals("1"))
            {
                A123PayBll.CompleteSuccess(transactionID, time, ticket, gws_rd);
            }
            else
            {
                A123PayBll.TransactionCancelOrReject(transactionID, time, ticket, gws_rd);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult A123ErrorOrCancelCallback(string transactionID, string time, string status, string ticket, string gws_rd)
        {
            A123PayBll.TransactionCancelOrReject(transactionID, time, ticket, gws_rd);
            return RedirectToAction("Index", "Home");
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        public ActionResult SendNewOrderDelivery(List<OrderDetail> orderDetails, long orderid)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Common/TestOrder.json")))
            {
                body = reader.ReadToEnd();
            }

            #region Tao body giong voi file TestOrder.json
            var token = WebConfigurationManager.AppSettings["token"];
            string lstproduct = string.Empty;
            var lstOrder = OrderBll.GetOrderDetailByOrderId(orderid);
            int dem = 0;
            foreach (var item in lstOrder)
            {
                lstproduct += "{";
                lstproduct += "name" + ":"+ "\""+item.Product.Name +"\""+"," ;
                lstproduct += "quantity" + ":" + "\"" + item.Quantity + "\"";
                lstproduct += "}";
                dem++;
                if (dem <= lstOrder.Count - 1 && dem > 0) lstproduct += ",";
            }
            body = body.Replace("{token}", token);
            body = body.Replace("{lstproduct}", lstproduct);

            body = body.Replace("{id}","01");
            body = body.Replace("{name}", "Banh Chi Hong");
            body = body.Replace("{address}", "Bai Say");
            body = body.Replace("{province}", "HCM");
            body = body.Replace("{district}", "5");
            body = body.Replace("{ward}", "Doi");
            body = body.Replace("{pick_money}", "500000");
            body = body.Replace("{pick_name}", "Lo Gom");
            body = body.Replace("{pick_address}", "Cau Lo Gom");
            body = body.Replace("{pick_province}", "HCM");
            body = body.Replace("{pick_district}", "8");

            #endregion

            var deliveryConfig = DeliveryConfig.Instance;
            var responseStr = InlamiaHttpRequest.Post(body, deliveryConfig.DeliveryCreateOrderPostUrlTest);
            if (responseStr != null)
            {
                var response = JsonConvert.DeserializeObject<Reponse>(responseStr);
                TempData["SendOrderMessage"] = response.Message;
                return RedirectToAction("Update", new { id = orderid });
            }
            else
            {
                var PickAddress = new Models.GHTK.AddressCustom
                {
                    Address = "Phu Nu Mart",
                    District = "1",
                    Province = "TPHCM"
                };
                var order = OrderBll.FindOneById(orderid);
                var CustomerAddress = new Models.GHTK.AddressCustom
                {
                    Address = order.Account.Address,
                    District = "6",
                    Province = "TPHCM"
                };
                var orderDetailsModel = new List<OrderDetailModel>();
                foreach (var orderDetail in orderDetails)
                {
                    orderDetailsModel.Add(new OrderDetailModel
                    {
                        Id = orderDetail.Id,
                        ProductId = orderDetail.ProductId,
                        OrderId = orderDetail.OrderId,
                        ProductName = orderDetail.Product.Name,
                        Quantity = orderDetail.Quantity,
                        OriginUnitPrice = orderDetail.OriginUnitPrice,
                        Discount = orderDetail.Discount,
                        CategoryName = orderDetail.Product.Category.Name
                    });
                }
                TempData["DeliveryTransactionModel"] = DeliveryBll.CreateDeliveryTransactionBySupplier(order.TotalPrice, orderDetailsModel, PickAddress, CustomerAddress);
                return RedirectToAction("Update", new { id = orderid });
            }
        }

        [AuthorizeActionFilter(RoleEnum.Manager, RoleEnum.Admin)]
        [HttpPost]
        public ActionResult SendNewOrderDelivery(DeliveryTransactionModel deliveryTransactionModel, long orderid)
        {
            var result = DeliveryBll.SendDeliveryTransaction(deliveryTransactionModel);
            if (result != null && result.IsSucess == true)
            {
                TempData["SendOrderDeliveryIsSuccess"] = result.IsSucess;
                TempData["DeliveryTransactionModel"] = deliveryTransactionModel;
            }
            return RedirectToAction("Update", new { id = orderid });
        }

        [HttpPost]
        public JsonResult IsAllowCreateOrder()
        {
            var isAllow = OrderBll.IsAllowCreateOrder();

            return Json(new { IsAllow = isAllow });
        }
    }
}