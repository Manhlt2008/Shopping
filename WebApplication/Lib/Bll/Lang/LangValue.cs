using System;
using System.Collections.Generic;

namespace WebApplication.Lib.Bll.Lang
{
    public partial class LangValue
    {
        public enum LangEnum
        {
            Vi, Eng
        }

        public enum DictionaryEnum
        {
            MasterPage, Footer, CmsHomeIndex, CmsProductView, CmsProductCategory, CmsOrderCart,
            CmsAuthenticationLogin, CmsAuthenticationRegister, CmsAuthenticationForgotPassword
        }

        public Dictionary<string, string> ValueDictionary { get; set; }

        public DictionaryEnum CurrentDictionaryEnum { get; set; }
        public LangEnum CurrentLangEnum { get; set; }

        public LangValue(LangEnum langEnum = LangEnum.Vi, DictionaryEnum dictionaryEnum = DictionaryEnum.CmsHomeIndex)
        {
            CurrentDictionaryEnum = dictionaryEnum;
            CurrentLangEnum = langEnum;

            switch (langEnum)
            {
                case LangEnum.Vi:
                case LangEnum.Eng:
                default:
                    switch (dictionaryEnum)
                    {
                        case DictionaryEnum.CmsHomeIndex:
                            ValueDictionary = new Dictionary<string, string>
                            {
                                {CmsHome.Index.Title, "Trang Chủ"},
                                {CmsHome.Index.More, "Chi Tiết"},
                                {CmsHome.Index.Sale, "Sale"},
                                {CmsHome.Index.New, "New"},
                                {CmsHome.Index.AddToCart, "Thêm Vào Giỏ Hàng"},
                                {CmsHome.Index.AddCart, "Thêm"},
                                {CmsHome.Index.Categories, "Danh Mục Sản Phẩm"},
                                {CmsHome.Index.Special, "Đặc Biệt"}
                            };
                            break;
                        case DictionaryEnum.MasterPage:
                            ValueDictionary = new Dictionary<string, string>
                            {
                                {MasterPage.Menu.Home, "Trang Chủ"},
                                {MasterPage.Menu.ContactUs, "Liên Hệ"},
                                {MasterPage.Menu.AboutUs, "Về Chúng Tôi"},
                                {MasterPage.Menu.Newest, "Mới Nhất"},
                                {MasterPage.Menu.ShoppingCart, "Giỏ Hàng"},
                                {MasterPage.Menu.ShoppingCartView, "Xem Giỏ Hàng"},
                                {MasterPage.Menu.ShoppingCartCheckout, "Thanh Toán"},
                                {MasterPage.Menu.Account, "Tài Khoản"},
                                {MasterPage.Menu.AccountLogin, "Đăng Nhập"},
                                {MasterPage.Menu.AccountRegister, "Đăng Ký"},
                                {MasterPage.Menu.AccountLogout, "Đăng Xuất"},
                                {MasterPage.Menu.Search, "Tìm Kiếm"},
                                {MasterPage.Menu.SearchPlaceHolder, "Nhập Từ Khóa"},
                                {MasterPage.Menu.BestSeller, "Bán Chạy"},
                                {MasterPage.Menu.Stand, "Gian Hàng"}
                            };
                            break;
                        case DictionaryEnum.Footer:
                            ValueDictionary = new Dictionary<string, string>
                            {
                                {MasterPage.Footer.InfoTitle, "Thông Tin"},
                                {MasterPage.Footer.PrivacyPolicy, "Chính Sách Bảo Mật"},
                                {MasterPage.Footer.TermAndConditions, "Điều Khoản Sử Dụng"},
                                {MasterPage.Footer.Faq, "FAQ"},
                                {MasterPage.Footer.Return, "Chính Sách Hoàn Trả"},
                                {MasterPage.Footer.Operating, "Quy Chế Hoạt Động"},
                                {MasterPage.Footer.DeliveryInformation, "Chính Sách Vận Chuyển"}
                            };
                            break;
                        case DictionaryEnum.CmsProductView:
                            ValueDictionary = new Dictionary<string, string>
                            {
                                {CmsProduct.View.Title, "Xem Chi Tiết Sản Phẩm"},
                                {CmsProduct.View.Home, "Trang Chủ"},
                                {CmsProduct.View.Price, "Giá"},
                                {CmsProduct.View.Information, "Thông Tin"},
                                {CmsProduct.View.AddToCart, "Thêm Vào Giỏ Hàng"},
                                {CmsProduct.View.Category, "Danh Mục"},
                                {CmsProduct.View.Description, "Mô Tả Chi Tiết"},
                                {CmsProduct.View.Reviews, "Đánh Giá"},
                                {CmsProduct.View.ProductDescription, "Chi Tiết Sản Phẩm"},
                                {CmsProduct.View.ProductRelated, "Sản phẩm liên quan"}
                            };
                            break;
                        case DictionaryEnum.CmsProductCategory:
                            ValueDictionary = new Dictionary<string, string>
                            {
                                {CmsProduct.Category.Title, "Danh Sách Sản Phẩm"},
                                {CmsProduct.Category.Home, "Trang Chủ"},
                                {CmsProduct.Category.FilterBy, "Tìm Kiếm Bởi"},
                                {CmsProduct.Category.Display, "Hiển Thị"}
                            };
                            break;
                        case DictionaryEnum.CmsOrderCart:
                            ValueDictionary = new Dictionary<string, string>
                            {
                                {CmsOrder.Cart.Title, "Giỏ Hàng"},
                                {CmsOrder.Cart.ShoppingCart, "Giỏ Hàng"},
                                {CmsOrder.Cart.CartSubTotal, "Thành Tiền"},
                                {CmsOrder.Cart.Tax, "10% VAT"},
                                {CmsOrder.Cart.CartTotal, "Tổng Cộng"},

                                {CmsOrder.Cart.OrderTableHeadItem, "Sản Phẩm"},
                                {CmsOrder.Cart.OrderTableHeadPrice, "Đơn Giá"},
                                {CmsOrder.Cart.OrderTableHeadQuantity, "Số Lượng"},
                                {CmsOrder.Cart.OrderTableHeadTotal, "Thành Tiền"},

                                {CmsOrder.Cart.ProcessCheckout, "Thanh Toán"},

                                { CmsOrder.Cart.Home, "Trang Chủ"}
                            };
                            break;
                        case DictionaryEnum.CmsAuthenticationLogin:
                            ValueDictionary = new Dictionary<string, string>
                            {
                                {CmsAuthentication.Login.Title, "Đăng Nhập" },
                                {CmsAuthentication.Login.SignInTitle, "Đăng Nhập Vào Hệ Thống" },
                                {CmsAuthentication.Login.SignInTitleSub, "Sử dụng tài khoản đã có để đăng nhập vào hệ thống." },
                                {CmsAuthentication.Login.Email, "Email" },
                                {CmsAuthentication.Login.Password, "Mật Khẩu" },
                                {CmsAuthentication.Login.SignInNow, "Đăng Nhập" },
                                {CmsAuthentication.Login.WelcomeTo, "Chào mừng bạn đăng nhập vào PhuNuMart..." },
                                {CmsAuthentication.Login.WelcomeToSub, "Đăng nhập vào hệ thống PhuNuMart để nhận được những ưu đãi hấp dẫn nhất." },
                                {CmsAuthentication.Login.CreateAccount, "Đăng Ký Tài Khoản Mới" },
                                {CmsAuthentication.Login.ForgotPassword, "Quên Mật Khẩu" }
                            };
                            break;
                        case DictionaryEnum.CmsAuthenticationRegister:
                            ValueDictionary = new Dictionary<string, string>
                            {
                                {CmsAuthentication.Register.Title, "Đăng Ký"},
                                {CmsAuthentication.Register.CreateAnAccount, "Tạo Mới Tài Khoản"},
                                {
                                    CmsAuthentication.Register.CreateAnAccountSub,
                                    "Tạo một tài khoản để sử dụng tốt nhất các dịch vụ của hệ thống."
                                },
                                {CmsAuthentication.Register.AgreeTo, "Tôi đã đọc và đồng ý" },
                                {CmsAuthentication.Register.TermAndCondition, "Điều khoản sử dung" },
                                {CmsAuthentication.Register.SignUpNow, "Đăng Ký" },

                                {CmsAuthentication.Register.WelcomeTo, "Chào mừng bạn đăng nhập vào PhuNuMart..." },
                                {CmsAuthentication.Register.WelcomeToSub, "Đăng nhập vào hệ thống PhuNuMart để nhận được những ưu đãi hấp dẫn nhất." },

                                {CmsAuthentication.Register.FirstName, "Họ" },
                                {CmsAuthentication.Register.LastName, "Tên" },
                                {CmsAuthentication.Register.YourEmail, "Email" },
                                {CmsAuthentication.Register.YourPassword, "Mật khẩu" },
                                {CmsAuthentication.Register.ConfirmPassword, "Xác nhận mật khẩu" },
                                {CmsAuthentication.Register.PhoneNumber, "Số điện thoại" }
                            };
                            break;
                        case DictionaryEnum.CmsAuthenticationForgotPassword:
                            ValueDictionary = new Dictionary<string, string>
                            {
                                {CmsAuthentication.ForgotPassword.Title, "Quên Mật Khẩu" },
                                {CmsAuthentication.ForgotPassword.Description, "Hệ thống sẽ gửi một email để giúp bạn lấy lại mật khẩu" },
                                {CmsAuthentication.ForgotPassword.SendMeNow, "Lấy lại mật khẩu" },
                                {CmsAuthentication.ForgotPassword.ContactOnlineSupport, "Liên hệ với BQT" },
                                {CmsAuthentication.ForgotPassword.ModalTitle, "Gửi yêu cầu quên mật khẩu" },
                                {CmsAuthentication.ForgotPassword.ModalBody, "<p>Bạn đã gửi yêu cầu quên mật khẩu thành công</p><p>Email có hiệu lực trong vòng 24h.</p>" }
                            };
                            break;
                    }
                    break;
            }
        }

        public string GetValue(string key)
        {
            try
            {
                return ValueDictionary[key];
            }
            catch
            {
                throw new Exception("Key not found.", new Exception($"In {CurrentLangEnum}, {CurrentDictionaryEnum}, key \"{key}\" cannot be found."));
            }
        }
    }
}