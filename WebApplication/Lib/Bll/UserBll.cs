using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util;
using WebApplication.Lib.Util.Common;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.User;

namespace WebApplication.Lib.Bll
{
    public static class UserBll
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public const int FEATURED_DEFAULT_LIMIT = 5;
        public const int LATEST_DEFAULT_LIMIT = 5;

        public const string SORT_DEFAULT = "default";
        public const string SORT_EMAIL = "Email";
        public const string SORT_FULL_NAME = "Fullname";
        public const string SORT_PHONE = "Phone";
        public const string SORT_ADDRESS = "Address";

        public const int ASC = 1;
        public const int DESC = 0;

        public const int STATUS_ALL = -1;

        public static ResultModel CreateUser(UserModel userModel, int? role = RoleEnum.User)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                using (var dbContext = new Entities())
                {
                    #region [Validate]
                    // 1. Email, Passowrd, Firstname, Lastname không được phép trống
                    if (string.IsNullOrEmpty(userModel.Email))
                    {
                        Log.Error("Email cannot be empty");
                        resultModel.setCode(Result.ACCOUNT_EMAIL_IS_EMPTY);

                        return resultModel;
                    }

                    if (string.IsNullOrEmpty(userModel.Password))
                    {
                        Log.Error("Password cannot be empty");
                        resultModel.setCode(Result.ACCOUNT_PASSWORD_IS_EMPTY);

                        return resultModel;
                    }

                    if (string.IsNullOrEmpty(userModel.Firstname))
                    {
                        Log.Error("Firstname cannot be empty");
                        resultModel.setCode(Result.ACCOUNT_FIRSTNAME_IS_EMPTY);

                        return resultModel;
                    }

                    if (string.IsNullOrEmpty(userModel.Lastname))
                    {
                        Log.Error("Lastname cannot be empty");
                        resultModel.setCode(Result.ACCOUNT_LASTNAME_IS_EMPTY);

                        return resultModel;
                    }

                    // 2. Format Email
                    if (!Utils.IsValidEmail(userModel.Email))
                    {
                        Log.Error("Email is not valid format");
                        resultModel.setCode(Result.ACCOUNT_EMAIL_INVALID_FORMAT);

                        return resultModel;
                    }

                    // 3. Validation password length
                    if (userModel.Password.Length < Constant.PASSWORD_MIN_LENGTH || userModel.Password.Length > Constant.PASSWORD_MAX_LENGTH)
                    {
                        Log.Info("Password length must be between" + Constant.PASSWORD_MIN_LENGTH + " and " + Constant.PASSWORD_MAX_LENGTH);
                        resultModel.setCode(Result.ACCOUNT_PASSWORD_INVALID_LENGTH);

                        return resultModel;
                    }

                    // 4. Email must be unique
                    var acc = dbContext.Accounts.FirstOrDefault(m => m.Email.ToLower().Equals(userModel.Email.Trim().ToLower()));
                    if (acc != null)
                    {
                        Log.Info("Email cannot be duplicated");
                        resultModel.setCode(Result.ACCOUNT_EMAIL_IS_EXISTED);

                        return resultModel;
                    }

                    #endregion

                    if (role != null)
                    {
                        string md5Password = Utils.HashMD5(userModel.Password);
                        Account account = new Account
                        {
                            Email = userModel.Email.Trim().ToLower(),
                            Password = md5Password,
                            Firstname = userModel.Firstname.Trim(),
                            Lastname = userModel.Lastname.Trim(),
                            JoinDate = DateTime.Now,
                            RoleId = role.Value,
                            DateOfBirth = DateTime.Now,
                            Status = StatusEnum.Active
                        };

                        dbContext.Accounts.Add(account);
                        if (dbContext.SaveChanges() > 0)
                        {
                            resultModel.Data = account;
                            resultModel.setCode(Result.SUCCESS);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("CreateUser", exception);
                resultModel.setCode(Result.FAILED);
            }

            return resultModel;
        }

        public static ResultModel Login(string email, string password)
        {
            string md5Password = Utils.HashMD5(password);
            ResultModel resultModel = new ResultModel();
            try
            {
                using (var dbContext = new Entities())
                {
                    #region [Login]
                    var acc = dbContext.Accounts.FirstOrDefault(m => m.Email.ToLower().Equals(email.ToLower())
                                                                && m.Password.Equals(md5Password)
                                                                && m.Status == StatusEnum.Active);
                    if (acc != null)
                    {
                        resultModel.setCode(Result.SUCCESS);
                        HttpContext.Current.Session[MvcApplication.User] = acc.Id;
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
                Log.Error("Login", exception);
                resultModel.setCode(Result.FAILED);
            }

            return resultModel;
        }

        public static ResultModel ChangePassword(UserModel currentUser, string email, string passwordOld, string passwordNew, string passwordConfirm)
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
                    var accAdmin = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().ToLower().Equals(currentUser.Email.Trim().ToLower()) && (m.RoleId.Equals(RoleEnum.Admin) || m.RoleId.Equals(RoleEnum.Manager)));
                    var acc = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().ToLower().Equals(email.Trim().ToLower()));
                    if (accAdmin == null) // Nếu không phải tài khoản Admin - Manager thì kiểm tra mật khẩu cũ
                    {
                        if (passwordOld == null)
                        {
                            resultModel.setCode(Result.ACCOUNT_PASSWORD_IS_EMPTY);
                            return resultModel;
                        }

                        var md5PasswordOld = Utils.HashMD5(passwordOld);

                        acc = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().ToLower().Equals(email.Trim().ToLower()) && m.Password.Trim().Equals(md5PasswordOld.Trim()));

                    }

                    if (passwordNew == null || passwordConfirm == null)
                    {
                        resultModel.setCode(Result.ACCOUNT_PASSWORD_IS_EMPTY);
                        return resultModel;
                    }

                    if (acc == null)
                    {
                        Log.Info("Passowrd Old Incorrect");
                        resultModel.setCode(Result.ACCOUNT_INCORRECT_PASSWORD_OLD);

                        return resultModel;
                    }
                    if (passwordNew.Trim().Length < Constant.PASSWORD_MIN_LENGTH || passwordNew.Trim().Length > Constant.PASSWORD_MAX_LENGTH)
                    {
                        Log.Info("Passowrd length between" + Constant.PASSWORD_MIN_LENGTH + " and " + Constant.PASSWORD_MAX_LENGTH);
                        resultModel.setCode(Result.ACCOUNT_PASSWORD_INVALID_LENGTH);

                        return resultModel;
                    }

                    if (!passwordNew.Trim().Equals(passwordConfirm.Trim()))
                    {
                        Log.Info("Passowrd New and Password New confirm is not as same!");
                        resultModel.setCode(Result.ACCOUNT_PASSWORD_IS_DIFFERENT);

                        return resultModel;
                    }
                    #endregion

                    #region [ChangePassword]
                    if (acc != null)
                    {
                        var md5PasswordNew = Utils.HashMD5(passwordNew);

                        acc.Password = md5PasswordNew.Trim();

                        dbContext.SaveChanges();

                        acc.Password = null;
                        resultModel.Data = acc;
                        resultModel.setCode(Result.SUCCESS);
                    }
                    #endregion
                }
            }
            catch (Exception exception)
            {
                Log.Error("ChangePassword", exception);
                resultModel.setCode(Result.FAILED);
            }

            return resultModel;
        }

        public static ResultModel UpdateInfo(UserModel currentUser, UserModel model)
        {
            var resultModel = new ResultModel();
            var userModel = new UserModel();

            try
            {
                using (var dbContext = new Entities())
                {
                    #region [Validation]
                    var accAdmin = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().ToLower().Equals(currentUser.Email.Trim().ToLower())
                                                                        && m.RoleId.Equals(RoleEnum.Admin));
                    var acc = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().ToLower().Equals(model.Email.Trim().ToLower()));
                    if (acc == null || (accAdmin == null && acc != null))
                    {
                        Log.Info("Lỗi xác thực");
                        resultModel.setCode(Result.AUTH);

                        return resultModel;
                    }

                    if (model.Firstname.Trim().Equals(string.Empty))
                    {
                        Log.Info("Firstname cannot be empty");
                        resultModel.setCode(Result.ACCOUNT_FIRSTNAME_IS_EMPTY);

                        return resultModel;
                    }

                    if (model.Lastname.Trim().Equals(string.Empty))
                    {
                        Log.Info("Lastname cannot be empty");
                        resultModel.setCode(Result.ACCOUNT_LASTNAME_IS_EMPTY);

                        return resultModel;
                    }
                    #endregion

                    #region [Update Info]
                    if (acc != null)
                    {
                        acc.Firstname = model.Firstname;
                        acc.Lastname = model.Lastname;
                        acc.Gender = model.Gender;
                        acc.DateOfBirth = model.DateOfBirth;
                        acc.Phone = model.Phone;
                        acc.Province = model.Province;
                        acc.District = model.District;
                        acc.Ward = model.Ward;
                        acc.Address = model.Address;

                        dbContext.SaveChanges();

                        acc.Password = null;
                        resultModel.Data = acc;
                        resultModel.setCode(Result.SUCCESS);
                        if (accAdmin == null)
                        {
                            HttpContext.Current.Session[MvcApplication.User] = model;
                        }
                    }
                    #endregion
                }
            }
            catch (Exception exception)
            {
                Log.Error("UpdateInfo", exception);
                resultModel.setCode(Result.FAILED);
            }

            return resultModel;
        }

        public static UserModel GetUser()
        {
            long? userId = (long?)HttpContext.Current.Session[Constant.USER];

            if (userId == null)
            {
                return null;
            }

            ResultModel resultModel = new ResultModel();
            UserModel userModel = null;
            try
            {
                using (var dbContext = new Entities())
                {
                    var user = dbContext.Accounts.FirstOrDefault(account => account.Id == userId);
                    if (user != null)
                    {
                        userModel = new UserModel();

                        userModel.Id = user.Id;
                        userModel.Email = user.Email;
                        userModel.Password = null;
                        userModel.Firstname = user.Firstname;
                        userModel.Lastname = user.Lastname;
                        userModel.Phone = user.Phone;
                        userModel.Address = user.Address;
                        userModel.DateOfBirth = (DateTime)user.DateOfBirth;
                        userModel.Gender = user.Gender;
                        userModel.RoleId = user.RoleId;
                        userModel.Province = user.Province;
                        userModel.District = user.District;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("GetUser", exception);
                resultModel.setCode(Result.FAILED);
            }

            return userModel;
        }

        public static ResultModel ForgotPassword(string email, StreamReader reader)
        {
            var resultModel = new ResultModel();
            var userModel = new UserModel();

            try
            {
                using (var dbContext = new Entities())
                {
                    #region [Validation]

                    if (email.Trim().Equals(string.Empty))
                    {
                        Log.Info("Email cannot be empty");
                        resultModel.setCode(Result.ACCOUNT_EMAIL_IS_EMPTY);

                        return resultModel;
                    }

                    if (!Utils.IsValidEmail(email.Trim()))
                    {
                        Log.Info("Email is not valid format");
                        resultModel.setCode(Result.ACCOUNT_EMAIL_INVALID_FORMAT);

                        return resultModel;
                    }

                    var acc = dbContext.Accounts.FirstOrDefault(m => m.Email == email.Trim());
                    if (acc == null)
                    {
                        Log.Info("Email does not existed!");
                        resultModel.setCode(Result.ACCOUNT_EMAIL_IS_NOT_EXISTED);

                        return resultModel;
                    }
                    #endregion

                    #region [Update Forgot password token]
                    if (acc != null)
                    {
                        var forgotPasswordToken = Utils.createToken();
                        acc.ForgotPasswordToken = forgotPasswordToken;
                        acc.TokenCreatedDate = DateTime.Now;

                        dbContext.SaveChanges();
                        var content = Utils.createLinkForgotPassword(email, forgotPasswordToken);
                        string body = string.Empty;
                        body = reader.ReadToEnd();
                        body = body.Replace("{link}", content);
                        body = body.Replace("{date}", DateTime.Now.ToString());
                        SendMail.SendEmail(Email.CompanyEmail, Email.CompanyPasswordEmail, email, Email.TitleForgotPassword, body);
                        resultModel.Data = null;
                        resultModel.setCode(Result.SUCCESS);
                    }
                    #endregion
                }
            }
            catch (Exception exception)
            {
                Log.Error("ForgotPassword", exception);
                resultModel.setCode(Result.FAILED);
            }

            return resultModel;
        }

        public static ResultModel isValidTokenForgotPassword(string email, string token)
        {
            var resultModel = new ResultModel();
            var userModel = new UserModel();

            try
            {
                using (var dbContext = new Entities())
                {

                    var acc = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().Equals(email.Trim()) && m.ForgotPasswordToken.Trim().Equals(token.Trim()));
                    if (acc == null)
                    {
                        Log.Info("Token or Email Inccorect!");
                        resultModel.setCode(Result.ACCOUNT_INCORRECT_TOKEN_FORGOT_PASSWORD);

                        return resultModel;
                    }
                    else
                    {
                        DateTime tokenTime = (DateTime)acc.TokenCreatedDate;
                        DateTime currentTime = DateTime.Now;

                        tokenTime = tokenTime.AddDays(1);

                        if (tokenTime.CompareTo(currentTime) > 0)
                        {
                            resultModel.setCode(Result.SUCCESS);
                            resultModel.Data = acc;

                            return resultModel;
                        }
                        else
                        {
                            Log.Info("Token is Expired!");
                            resultModel.setCode(Result.ACCOUNT_TOKEN_EXPIRED);

                            return resultModel;
                        }
                    }


                }
            }
            catch (Exception exception)
            {
                Log.Error("ForgotPassword", exception);
                resultModel.setCode(Result.FAILED);
            }

            return resultModel;
        }

        public static ResultModel ResetPassword(string email, string token, string passwordNew, string passwordConfirm)
        {
            var resultModel = new ResultModel();
            var resultModel2 = new ResultModel();
            var userModel = new UserModel();

            try
            {
                using (var dbContext = new Entities())
                {
                    #region [Validation]
                    if (token == null)
                    {
                        resultModel.setCode(Result.ACCOUNT_TOKEN_NOT_FOUND);
                        return resultModel;
                    }

                    resultModel2 = isValidTokenForgotPassword(email, token);
                    if (resultModel2.Code != 0)
                    {
                        resultModel = resultModel2;
                        return resultModel;
                    }

                    if (passwordNew.Trim().Length < Constant.PASSWORD_MIN_LENGTH || passwordNew.Trim().Length > Constant.PASSWORD_MAX_LENGTH)
                    {
                        Log.Info("Passowrd length between" + Constant.PASSWORD_MIN_LENGTH + " and " + Constant.PASSWORD_MAX_LENGTH);
                        resultModel.setCode(Result.ACCOUNT_PASSWORD_INVALID_LENGTH);

                        return resultModel;
                    }

                    if (!passwordNew.Trim().Equals(passwordConfirm.Trim()))
                    {
                        Log.Info("Passowrd New and Password New confirm is not as same!");
                        resultModel.setCode(Result.ACCOUNT_PASSWORD_IS_DIFFERENT);

                        return resultModel;
                    }
                    #endregion

                    #region [ChangePassword]
                    var acc = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().Equals(email.Trim()) && m.ForgotPasswordToken.Trim().Equals(token.Trim()));
                    if (acc != null)
                    {
                        var md5PasswordNew = Utils.HashMD5(passwordNew);

                        acc.Password = md5PasswordNew.Trim();
                        acc.ForgotPasswordToken = null;
                        acc.TokenCreatedDate = null;
                        dbContext.SaveChanges();

                        acc.Password = null;
                        resultModel.Data = acc;
                        resultModel.setCode(Result.SUCCESS);
                    }
                    #endregion
                }
            }
            catch (Exception exception)
            {
                Log.Error("Reset Password", exception);
                resultModel.setCode(Result.FAILED);
            }

            return resultModel;
        }

        public static ResultModel ListAllUser(UserModel currentUser, string searchKey, string searchValue, int status)
        {
            var resultModel = new ResultModel();
            var userModel = new UserModel();
            try
            {
                using (var dbContext = new Entities())
                {
                    #region [Validation]

                    #endregion

                    #region [ListAllUser]
                    var acc = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().Equals(currentUser.Email.Trim())
                                                                && m.RoleId == RoleEnum.Admin);
                    if (acc != null)
                    {
                        var listAcc = dbContext.Accounts.ToList();

                        string email = "";
                        string address = "";
                        string phone = "";
                        string fullname = "";

                        if (searchValue == null || searchValue.Trim().Length == 0)
                            searchValue = "";
                        else
                            searchValue = searchValue.Trim().ToLower();

                        switch (searchKey)
                        {
                            case "email":
                                email = searchValue;
                                break;
                            case "address":
                                address = searchValue;
                                break;
                            case "phone":
                                phone = searchValue;
                                break;
                            case "fullname":
                                fullname = searchValue;
                                break;
                        }
                        bool isInputNull = false;

                        if (email.Equals("") && address.Equals("") && phone.Equals("") && fullname.Equals(""))
                        {
                            isInputNull = true;
                        }

                        if (isInputNull) // Khi không có dữ liệu thì load hết theo status
                        {
                            if (status != STATUS_ALL) // Lấy theo status
                            {
                                listAcc = dbContext.Accounts.Where(m => (m.Status.Equals(status) && !m.RoleId.Equals(RoleEnum.Admin)))
                                    .ToList();
                            }
                            else // lấy tất cả
                            {
                                listAcc = dbContext.Accounts.Where(m => (!m.RoleId.Equals(RoleEnum.Admin))).ToList();
                            }
                        }
                        else // Khi có dữ liệu thì load theo thuộc tính
                        {
                            if (status != STATUS_ALL) // Lấy theo status
                            {
                                listAcc = dbContext.Accounts.Where(m => (m.Email.ToLower().Contains(email.ToLower().Trim())
                                                                   || m.Firstname.ToLower().Contains(fullname.ToLower().Trim())
                                                                   || m.Lastname.ToLower().Contains(fullname.ToLower().Trim())
                                                                   || m.Phone.ToLower().Contains(phone.ToLower().Trim())
                                                                   || m.Address.ToLower().Contains(address.ToLower().Trim()))
                                                                   && m.Status.Equals(status)
                                                                   && !m.RoleId.Equals(RoleEnum.Admin)).ToList();
                            }
                            else // lấy tất cả
                            {
                                listAcc = dbContext.Accounts.Where(m => (m.Email.ToLower().Contains(email.ToLower().Trim())
                                                                   || m.Firstname.ToLower().Contains(fullname.ToLower().Trim())
                                                                   || m.Lastname.ToLower().Contains(fullname.ToLower().Trim())
                                                                   || m.Phone.ToLower().Contains(phone.ToLower().Trim())
                                                                   || m.Address.ToLower().Contains(address.ToLower().Trim()))
                                                                   && !m.RoleId.Equals(RoleEnum.Admin)).ToList();
                            }
                        }

                        resultModel.setCode(Result.SUCCESS);
                        resultModel.Data = listAcc;

                    }
                    #endregion
                }
            }
            catch (Exception exception)
            {
                Log.Error("Reset Password", exception);
                resultModel.setCode(Result.FAILED);
            }

            return resultModel;
        }

        public static ResultModel UpdateStatusAccount(UserModel currentUser, string email, int status)
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
                    if (status != StatusEnum.InActive && status != StatusEnum.Active)
                    {
                        resultModel.setCode(Result.INVALID_DATA);
                        return resultModel;
                    }
                    #endregion

                    #region [ListAllUser]
                    var accountAdmin = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().Equals(currentUser.Email.Trim())
                                                                && m.RoleId == RoleEnum.Admin);
                    if (accountAdmin != null)
                    {
                        // Không được thay đổi status account admin
                        var acc = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().Equals(email.Trim())
                                                                && !m.RoleId.Equals(RoleEnum.Admin));
                        if (acc != null)
                        {
                            acc.Status = status;
                            dbContext.SaveChanges();
                            resultModel.setCode(Result.SUCCESS);
                            resultModel.Data = acc;
                        }
                        else
                        {
                            resultModel.setCode(Result.ACCOUNT_IS_ADMIN);
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

        internal static List<Account> Search(string query)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var nativeSqlQuery =
                        "SELECT * FROM Account WHERE Email LIKE @query OR (Firstname + ' ' + Lastname) LIKE @query OR Id LIKE @query OR Phone LIKE @query";

                    return dbContext.Accounts.SqlQuery(nativeSqlQuery,
                        new SqlParameter("@query", "%" + StringUtil.RemoveVietnameseTone(
                                    StringUtil.RemoveSign4VietnameseString(query.ToLower().Trim())) + "%")).ToList();

                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }

            return Enumerable.Empty<Account>().ToList();
        }

        public static bool isExistedEmail(string email)
        {
            var resultModel = new ResultModel();
            var userModel = new UserModel();
            try
            {
                using (var dbContext = new Entities())
                {
                    #region [Validation]
                    #endregion

                    #region [ListAllUser]

                    var account = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().Equals(email.Trim()));
                    if (account != null)
                    {
                        userModel.Email = account.Email;
                        userModel.Firstname = account.Firstname;
                        userModel.Lastname = account.Lastname;
                        userModel.Gender = account.Gender;
                        userModel.DateOfBirth = (DateTime)account.DateOfBirth;
                        userModel.Phone = account.Phone;
                        userModel.Address = account.Address;

                        return true;
                    }
                    #endregion
                }
            }
            catch (Exception exception)
            {
                Log.Error("GetAccountByEmail", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            resultModel.Data = userModel;

            return false;
        }

        public static ResultModel GetAccountByEmail(UserModel currentUser, string email)
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
                        resultModel.Data = userModel;

                        return resultModel;
                    }
                    #endregion

                    #region [ListAllUser]
                    var acc = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().Equals(currentUser.Email.Trim())
                                                                && m.RoleId == RoleEnum.Admin || m.RoleId == RoleEnum.Manager);
                    if (acc != null)
                    {
                        var account = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().Equals(email.Trim()));
                        if (account != null)
                        {
                            userModel.Email = account.Email;
                            userModel.Firstname = account.Firstname;
                            userModel.Lastname = account.Lastname;
                            userModel.Gender = account.Gender;
                            userModel.DateOfBirth = (DateTime)account.DateOfBirth;
                            userModel.Phone = account.Phone;
                            userModel.Address = account.Address;

                            resultModel.setCode(Result.SUCCESS);
                        }
                        else
                        {
                            resultModel.setCode(Result.ACCOUNT_EMAIL_IS_NOT_EXISTED);
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
                Log.Error("GetAccountByEmail", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            resultModel.Data = userModel;

            return resultModel;
        }

        public static ResultModel GetAccountById(UserModel currentUser, long userId)
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
                        resultModel.Data = userModel;

                        return resultModel;
                    }

                    #endregion

                    #region [ListAllUser]

                    var acc = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().Equals(currentUser.Email.Trim())
                                                                     && m.RoleId == RoleEnum.Admin ||
                                                                     m.RoleId == RoleEnum.Manager);
                    if (acc != null)
                    {
                        var account = dbContext.Accounts.FirstOrDefault(m => m.Id == userId);
                        if (account != null)
                        {
                            userModel.Email = account.Email;
                            userModel.Firstname = account.Firstname;
                            userModel.Lastname = account.Lastname;
                            userModel.Gender = account.Gender;
                            userModel.DateOfBirth = (DateTime)account.DateOfBirth;
                            userModel.Phone = account.Phone;
                            userModel.Province = account.Province;
                            userModel.District = account.District;
                            userModel.Ward = account.Ward;
                            userModel.Address = account.Address;

                            resultModel.setCode(Result.SUCCESS);
                        }
                        else
                        {
                            resultModel.setCode(Result.ACCOUNT_EMAIL_IS_NOT_EXISTED);
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
                Log.Error("GetAccountByEmail", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            resultModel.Data = userModel;

            return resultModel;
        }
    }
}