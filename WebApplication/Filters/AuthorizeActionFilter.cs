using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using WebApplication.Lib.Bll;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.User;

namespace WebApplication.Filters
{
    public class AuthorizeActionFilter : ActionFilterAttribute
    {
        private readonly long[] _roles;
        public AuthorizeActionFilter(params long[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                var user = UserBll.GetUser();
                if (user == null)
                {
                    // Redirect to Login Page
                    FormsAuthentication.SignOut();

                    HttpContext.Current.Session["PreviousUrl"] = filterContext.HttpContext.Request.Url;

                    filterContext.Result = new RedirectResult("~/Authentication/Login");

                }
                else //nếu đang còn session
                {
                    var isAllowAccess = true;
                    if (_roles != null && _roles.Length > 0)
                    {
                        isAllowAccess = _roles.Contains(user.RoleId);
                    }

                    if (isAllowAccess)
                    {
                        HttpContext.Current.Session[Constant.USER] = user.Id;
                    }
                    else
                    {
                        filterContext.Result = new HttpStatusCodeResult(403);
                        throw new HttpException(403, "Access Denied");
                    }
                }
            }
        }
    }
}