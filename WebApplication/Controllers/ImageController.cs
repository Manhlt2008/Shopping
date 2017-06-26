using System;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
using log4net;
using WebApplication.Common;
using WebApplication.Lib.Bll;

namespace WebApplication.Controllers
{
    public class ImageController : Controller
    {
        //
        // GET: /Image/
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult View(long id)
        {
            var img = ImageBll.FindById(id);

            if (img.IsLoadLocal)
            {

                byte[] image = null;
                try
                {
                    image = System.IO.File.ReadAllBytes(Path.Combine(ImageBll.GetServerPath(), img.Path));
                }
                catch (Exception ex)
                {
                    try
                    {
                        _log.Error("View", ex);
                        image = System.IO.File.ReadAllBytes(Server.MapPath(Url.Content("~/Content/imageNotFound.jpg")));
                    }
                    catch (Exception exception)
                    {
                        _log.Error("View", exception);
                        image = null;
                    }
                }

                const string contentType = "image/jpeg";
                return this.Image(image, contentType);
            }

            return Redirect(img.Path);
        }
    }
}
