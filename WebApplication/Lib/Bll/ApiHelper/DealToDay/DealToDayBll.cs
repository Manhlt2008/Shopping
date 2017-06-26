using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Transactions;
using System.Web;
using System.Web.Script.Serialization;
using WebApplication.Lib.Dal.ConfigData;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util.Constant;
using WebApplication.Lib.Util.Security;
using WebApplication.Models.APIModel.DealToDay;
using WebApplication.Models.User;
using SupplierCategory = WebApplication.Lib.Dal.DbContext.SupplierCategory;

namespace WebApplication.Lib.Bll.ApiHelper.DealToDay
{
    public static partial class DealToDayBll
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static DealToDayCreateOrderResponse CreateOrder(Order order)
        {
            try
            {
                List<DealToDayProduct> products = new List<DealToDayProduct>();
                DealToDayProduct product = new DealToDayProduct();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var dealToDayConfig = DealToDayConfig.Instance;
                order.TotalPrice = 0;
                foreach (var item in order.OrderDetails)
                {
                    product = new DealToDayProduct
                    {
                        DealId = serializer.Deserialize<DealToDayEInfoResponse>(item.Product.DealToDayInfo).result.dealId,
                        Quantity = item.Quantity,
                        UnitPrice = (int)(item.OriginUnitPrice * item.Discount)
                    };
                    order.TotalPrice += product.UnitPrice * product.Quantity;
                    products.Add(product);
                }
                var provinces = AddressBll.GetAllProvinces().OrderBy(m => m.Name);
                var orderProvince = provinces.First(m => m.ProvinceId == order.Province);
                var districts = AddressBll.GetDistrictsByProvince(order.Province).OrderBy(m => m.Name);
                var orderDistrict = districts.First(m => m.DistrictId == order.District);
                DealToDayCreateOrder createOrder = new DealToDayCreateOrder
                {
                    Cmd = DealToDayCreateOrder.cmd,
                    PartnerCode = DealToDayCreateOrder.partnerCode,
                    Timestamp = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    CreatedDate = order.CreatedDate.ToString("yyyyMMddHHmmss"),
                    CustomerAddress = (order.Address + " / " + orderDistrict + " / " + orderProvince).ToString(),
                    CustomerEmail = order.Account.Email,
                    CustomerFullName = order.Account.Firstname + " " + order.Account.Lastname,
                    CustomerGender = order.Account.Gender,
                    CustomerMobile = order.Account.Phone,
                    PaymentStatus = 2,
                    PaymentName = "Thanh Toan Truc Tiep",
                    TotalAmount = (int)order.TotalPrice,
                    OrderCode = order.Code,
                    Products = products,
                };
                var responseStr = InlamiaHttpRequest.InlamiaHttpRequest.Post(createOrder.getDictionary(), dealToDayConfig.ServiceUrl);
                if (responseStr != null)
                {
                    string jsonStr = serializer.Deserialize<string>(responseStr);
                    var response = serializer.Deserialize<DealToDayCreateOrderResponse>(jsonStr);
                    using (var dbContext = new Entities())
                    {
                        if ("00".Equals(response.errorCode))
                        {
                            foreach (var item in order.OrderDetails)
                            {
                                var orderDetails = dbContext.OrderDetails.First(m => m.Id == item.Id);
                                orderDetails.DealToDayOrderId = response.result.TransactionId;
                                dbContext.SaveChanges();
                            }
                            return response;
                        }
                        else
                        {
                            return null;
                        }

                    }
                }

            }
            catch (Exception exception)
            {
                Log.Error("DealToDayCreateOrderResponse()", exception);
            }
            return null;
        }

        public static DealToDayCommonResponse GetAllEDeal()
        {
            var dealToDayConfig = DealToDayConfig.Instance;

            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            var request = new DealToDayRequest
            {
                DealId = null,
                Cmd = DealToDayRequest.GetAllEDeal,
                PartnerCode = dealToDayConfig.PartnerCode,
                Timestamp = timestamp,
                Signature = HashingUtils.CreateSha256Token(DealToDayRequest.GetAllEDeal + timestamp + dealToDayConfig.PartnerCode, dealToDayConfig.Signature)
            };

            try
            {
                var responseStr = InlamiaHttpRequest.InlamiaHttpRequest.Post(request.GetDictionary(), dealToDayConfig.ServiceUrl);

                if (responseStr != null)
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string jsonStr = serializer.Deserialize<string>(responseStr);

                    using (var dbContext = new Entities())
                    {
                        var dealIds = dbContext.DealToDayCaches.Select(m => m.DealId).ToList();

                        var responsese = serializer.Deserialize<DealToDayCommonResponse>(jsonStr);

                        var result = responsese.result.Where(dealToDayObjectResult => !dealIds.Contains(dealToDayObjectResult.dealId)).ToList();

                        responsese.result = result;

                        return responsese;

                    }

                }
            }
            catch (Exception exception)
            {
                Log.Error("GetAllEDeal()", exception);
            }

            return null;
        }

        public static Product AddByDealId(int dealId)
        {
            Product product = null;
            var dealToDayConfig = DealToDayConfig.Instance;

            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            var request = new DealToDayRequest
            {
                DealId = dealId,
                Cmd = DealToDayRequest.GetEDealInfo,
                PartnerCode = dealToDayConfig.PartnerCode,
                Timestamp = timestamp,
                Signature = HashingUtils.CreateSha256Token(DealToDayRequest.GetEDealInfo + timestamp + dealToDayConfig.PartnerCode, dealToDayConfig.Signature)
            };

            try
            {
                var responseStr = InlamiaHttpRequest.InlamiaHttpRequest.Post(request.GetDictionary(),
                    dealToDayConfig.ServiceUrl);

                DealToDayEInfoResponse response = null;

                if (responseStr != null)
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string jsonStr = serializer.Deserialize<string>(responseStr);

                    response = JsonConvert.DeserializeObject<DealToDayEInfoResponse>(jsonStr);
                }

                if (response != null && response.result != null)
                {
                    using (var dbContext = new Entities())
                    {
                        using (var scope = new TransactionScope())
                        {
                            var isCreateNewCateogry = false;
                            var result = response.result;

                            #region [DealToDay Root Category]
                            var category = dbContext.Categories.FirstOrDefault(m => m.Type == CategoryTypeEnum.DealToDay);
                            if (category == null)
                            {
                                category = new Category
                                {
                                    Name = "Deal To Day",
                                    Status = StatusEnum.Active,
                                    Type = CategoryTypeEnum.DealToDay
                                };

                                dbContext.Categories.Add(category);
                                dbContext.SaveChanges();

                                isCreateNewCateogry = true;
                            }
                            #endregion

                            #region [Supplier]
                            var supplier =
                                dbContext.Suppliers.FirstOrDefault(
                                    m => m.Type == SupplierTypeEnum.DealToDay && m.Status == StatusEnum.Active);

                            if (supplier == null)
                            {
                                var isCreateSupplierSuccess = SupplierBll.Create(new Models.Supplier.Supplier
                                {
                                    Address = "702. Tầng 7 Tòa nhà HEC - số 02, Ngõ 95 Chùa Bộc - Đống Đa - Hà Nội",
                                    CategoryIds = new List<long> { category.Id },
                                    Email = "hotro@dealtoday.vn",
                                    Facebook = "https://www.facebook.com/dealtoday.vn",
                                    Name = "Deal To Day",
                                    Phone = "(04) 3518.7799",
                                    Status = StatusEnum.Active,
                                    Website = "https://www.dealtoday.vn/",
                                    UserModel = new UserModel
                                    {
                                        Status = StatusEnum.Active,
                                        Address = "702. Tầng 7 Tòa nhà HEC - số 02, Ngõ 95 Chùa Bộc - Đống Đa - Hà Nội",
                                        Phone = "(04) 3518.7799",
                                        DateOfBirth = DateTime.Now,
                                        Gender = 0,
                                        Firstname = "DealToDay",
                                        Lastname = "DealToDay",
                                        Email = "hptro@dealtoday.vn",
                                        Password = "123456",
                                        RoleId = RoleEnum.SupplierManager
                                    },
                                    ExraInfo = string.Empty
                                }, SupplierTypeEnum.DealToDay);

                                if (!isCreateSupplierSuccess)
                                {
                                    return null;
                                }

                                supplier =
                                dbContext.Suppliers.FirstOrDefault(
                                    m => m.Type == SupplierTypeEnum.DealToDay && m.Status == StatusEnum.Active);

                                if (supplier == null)
                                {
                                    return null;
                                }

                                var supplierCategory =
                                    dbContext.SupplierCategories.First(
                                        m => m.SupplierId == supplier.Id && m.Status == StatusEnum.Active);

                                var isCreateProductSupplierSuccess =
                                    SupplierBll.CreatProductBySupplier(new Dal.DbContext.SupplierProduct
                                    {
                                        ProductId = product.Id,
                                        SupplierId = supplier.Id,
                                        SupplierCategoryId = supplierCategory.Id,
                                        CreatedSupplierAccountId = supplier.SupplierAccounts.First().Id,
                                        Status = StatusEnum.Active,
                                        ExtraInfo = string.Empty
                                    });
                                if (!isCreateProductSupplierSuccess)
                                {
                                    return null;
                                }
                            }
                            #endregion

                            #region [Supplier Category]
                            if (isCreateNewCateogry)
                            {
                                dbContext.SupplierCategories.Add(new SupplierCategory
                                {
                                    CategoryId = category.Id,
                                    Status = StatusEnum.Active,
                                    SupplierId = supplier.Id
                                });
                                dbContext.SaveChanges();
                            }
                            #endregion

                            #region [Category]

                            // Assign to child category
                            var childCategory =
                                dbContext.Categories.FirstOrDefault(
                                    m => m.Type == CategoryTypeEnum.DealToDay && m.Status == StatusEnum.Active && m.Name.ToLower().Equals(result.categoryName.ToLower()));

                            if (childCategory == null)
                            {
                                childCategory = new Category
                                {
                                    Name = result.categoryName,
                                    Status = StatusEnum.Active,
                                    Type = CategoryTypeEnum.DealToDay,
                                    ParentCategoryId = category.Id
                                };

                                dbContext.Categories.Add(childCategory);
                                dbContext.SaveChanges();

                                dbContext.SupplierCategories.Add(new SupplierCategory
                                {
                                    CategoryId = childCategory.Id,
                                    Status = StatusEnum.Active,
                                    SupplierId = supplier.Id
                                });
                                dbContext.SaveChanges();
                            }
                            #endregion

                            var existedProduct =
                                dbContext.DealToDayCaches.FirstOrDefault(
                                    m => m.DealId == result.dealId && m.Product.Status == StatusEnum.Active);

                            if (existedProduct == null)
                            {
                                // Add new product

                                #region [Product]

                                product = new Product
                                {
                                    CreatedDate = DateTime.Now,
                                    CategoryId = childCategory.Id,
                                    Name = result.dealName,
                                    DealToDayInfo = responseStr,
                                    Description = result.description,
                                    IsFeatured = false,
                                    Price = result.price,
                                    Quantity = result.quantity,
                                    ShortDescription = result.shortDescription,
                                    Status = StatusEnum.Active
                                };

                                dbContext.Products.Add(product);

                                dbContext.SaveChanges();
                                var imgaes = new List<Image>
                                {
                                    new Image
                                    {
                                        Title = result.dealName,
                                        CreatedDate = DateTime.Now,
                                        IsLoadLocal = false,
                                        Status = StatusEnum.Active,
                                        Path = result.avatar,
                                        Type = ImageTypeEnum.Cover,
                                        ProductId = product.Id
                                    }
                                };

                                foreach (var dealToDayObjectResultImage in result.lstOtherImage)
                                {
                                    imgaes.Add(new Image
                                    {
                                        Title = result.dealName,
                                        CreatedDate = DateTime.Now,
                                        IsLoadLocal = false,
                                        Status = StatusEnum.Active,
                                        Path = dealToDayObjectResultImage.imgGallerySrc,
                                        Type = ImageTypeEnum.Gallery,
                                        ProductId = product.Id
                                    });
                                }
                                dbContext.Images.AddRange(imgaes);
                                dbContext.SaveChanges();

                                #endregion

                                #region DealToDayCache

                                var dealtodayCache = new DealToDayCache
                                {
                                    ProductId = product.Id,
                                    Address = JsonConvert.SerializeObject(result.address),
                                    Avatar = result.avatar,
                                    CategoryId = result.categoryId,
                                    CategoryName = result.categoryName,
                                    Condition = result.condition,
                                    DealId = result.dealId,
                                    DealName = result.dealName,
                                    Description = result.description,
                                    ExpiredDate = result.expiredDate,
                                    LocationId = result.localtionId,
                                    LocationName = result.locationName,
                                    LstOtherImage = JsonConvert.SerializeObject(result.lstOtherImage),
                                    OriginalPrice = result.originalPrice,
                                    Price = result.price,
                                    Quantity = result.quantity,
                                    ShortDescription = result.shortDescription,
                                    StartDate = result.startDate
                                };

                                dbContext.DealToDayCaches.Add(dealtodayCache);
                                dbContext.SaveChanges();

                                #endregion

                                #region [Supplier Product]

                                var supplierProduct = new Dal.DbContext.SupplierProduct
                                {
                                    CreatedSupplierAccountId = supplier.SupplierAccounts.First().Id,
                                    Status = StatusEnum.Active,
                                    SupplierCategoryId = supplier.SupplierCategories.First(m => m.CategoryId == childCategory.Id).Id,
                                    ProductId = product.Id,
                                    SupplierId = supplier.Id,
                                    ExtraInfo = string.Empty,
                                };
                                dbContext.SupplierProducts.Add(supplierProduct);
                                dbContext.SaveChanges();

                                #endregion
                            }
                            else
                            {
                                // Update product

                                #region [Cached]

                                var cached = new DealToDayCache
                                {
                                    ProductId = existedProduct.ProductId,
                                    Address = JsonConvert.SerializeObject(result.address),
                                    Avatar = result.avatar,
                                    CategoryId = result.categoryId,
                                    CategoryName = result.categoryName,
                                    Condition = result.condition,
                                    DealId = result.dealId,
                                    DealName = result.dealName,
                                    Description = result.description,
                                    ExpiredDate = result.expiredDate,
                                    LocationId = result.localtionId,
                                    LocationName = result.locationName,
                                    LstOtherImage = JsonConvert.SerializeObject(result.lstOtherImage),
                                    OriginalPrice = result.originalPrice,
                                    Price = result.price,
                                    Quantity = result.quantity,
                                    ShortDescription = result.shortDescription,
                                    StartDate = result.startDate,
                                    Id = existedProduct.Id
                                };

                                dbContext.DealToDayCaches.AddOrUpdate(cached);
                                dbContext.SaveChanges();

                                #endregion

                                #region [Product]

                                var updateProduct = new Product
                                {
                                    Id = existedProduct.ProductId,
                                    CreatedDate = DateTime.Now,
                                    CategoryId = childCategory.Id,
                                    Name = result.dealName,
                                    DealToDayInfo = responseStr,
                                    Description = result.description,
                                    IsFeatured = false,
                                    Price = result.price,
                                    Quantity = result.quantity,
                                    ShortDescription = result.shortDescription,
                                    Status = StatusEnum.Active
                                };

                                dbContext.Products.AddOrUpdate(updateProduct);
                                dbContext.SaveChanges();

                                #endregion
                            }

                            scope.Complete();
                        }
                    }

                }

                return product;
            }
            catch (Exception exception)
            {
                Log.Error("GetAllEDeal()", exception);
            }

            return null;
        }

        public static string NormalizeJsonString(string jsonString)
        {
            if (jsonString == null || jsonString.Length < 1)
            {
                return null;
            }

            jsonString = jsonString.Trim();

            jsonString = System.Text.RegularExpressions.Regex.Unescape(jsonString);

            jsonString = jsonString.Replace(@"\", string.Empty);

            jsonString = jsonString.Replace("\r", string.Empty);

            jsonString = jsonString.Replace("\n", string.Empty);

            if (jsonString[0] == '"')
            {
                jsonString = jsonString.Remove(0, 1);
            }

            if (jsonString[jsonString.Length - 1] == '"')
            {
                jsonString = jsonString.Remove(jsonString.Length - 1);
            }

            jsonString = HttpUtility.JavaScriptStringEncode(jsonString);

            //const string CONDTION_FIELD = "\"condition\"";
            //const string AVATAR_FIELD = "\"avatar\"";

            //int index = jsonString.IndexOf(CONDTION_FIELD);
            //while (index >= 0)
            //{
            //    while (!AVATAR_FIELD.Equals(jsonString.Substring(index + 1, CONDTION_FIELD.Length)))
            //    {
            //        jsonString = jsonString.Remove(index, 1);
            //    }

            //    // Remove redundant comma
            //    jsonString = jsonString.Remove(index, 1);

            //    index = jsonString.IndexOf(CONDTION_FIELD);
            //}

            return jsonString;
        }
    }
}