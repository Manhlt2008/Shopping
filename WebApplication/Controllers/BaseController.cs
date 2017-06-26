using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            //if (!string.IsNullOrEmpty(InlamiaSessionPersister.Username))
            //{
            //    filterContext.HttpContext.User =
            //        new InlamiaPrincipal(
            //            new InlamiaIdentity(
            //                InlamiaSessionPersister.Username, InlamiaSessionPersister.IsAdmin));
            //}

            base.OnAuthorization(filterContext);
        }
    }
}
