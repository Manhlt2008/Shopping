using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Web;
using log4net;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util.Constant;
using Image = WebApplication.Lib.Dal.DbContext.Image;
using SystemImage = System.Drawing.Image;

namespace WebApplication.Lib.Bll
{
    public class ImageBll
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static string ImageDirectory = "Images";

        public static string GetServerPath()
        {
            var serverPath = HttpContext.Current.Server.MapPath($"~/{ImageDirectory}");

            if (serverPath != null) return serverPath;

            Log.Info("Image settings path not found.");
            serverPath = Environment.GetEnvironmentVariable("TEMP") ?? "C:\\";

            return serverPath;
        }

        public static Image Save(HttpPostedFileBase file, long? id, string extraFilename = "")
        {
            var serverPath = GetServerPath();

            if (file == null) return null;
            if (file.ContentLength <= 0) return null;
            try
            {
                var fileName = Path.GetFileName(file.FileName);

                if (fileName == null) return null;

                var type = fileName.Split('.').Last();
                var name = DateTime.Now.ToString("G");
                name = name.Replace(" ", string.Empty);
                name = name.Replace("/", string.Empty);
                name = name.Replace(":", string.Empty);
                name = name.Remove(name.Length - 2);

                var tmpPath = Path.GetRandomFileName();
                tmpPath = tmpPath.Replace(".", ""); // Remove period.
                tmpPath = tmpPath.Substring(0, 8);  // Return 8 character string

                var path = Path.Combine(serverPath, tmpPath + name + extraFilename + "." + type);
                file.SaveAs(path);

                return new Image
                {
                    CreatedDate = new DateTime(),
                    Path = tmpPath + name + extraFilename + "." + type,
                    Status = StatusEnum.Active,
                    Title = fileName,
                    ProductId = id,
                    IsLoadLocal = true
                };
            }
            catch (Exception exception)
            {
                Log.Error("Save", exception);
                return null;
            }
        }

        public static SystemImage Get(string path)
        {
            try
            {
                var serverPath = GetServerPath();

                path = Path.Combine(serverPath, path);
                return SystemImage.FromFile(path);
            }
            catch (Exception exception)
            {
                Log.Error("Get", exception);
                return null;
            }

        }

        public static Image FindById(long id)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    return dbContext.Images.FirstOrDefault(m => m.Id == id);
                }
            }
            catch (Exception exception)
            {
                Log.Error("FindById", exception);
            }

            return null;
        }

        #region [Get Path]
        public static string GetPath(long imageId)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var img = dbContext.Images.FirstOrDefault(m => m.Id == imageId);
                    if (img == null)
                    {
                        Log.Info(string.Format("Image with ID {0} not found.", imageId));
                    }

                    return GetPath(img.Path);
                }
            }
            catch (Exception exception)
            {
                Log.Error("GetPath", exception);
            }

            return string.Empty;
        }

        public static string GetPath(string path)
        {
            var serverPath = GetServerPath();
            if (!path.Equals(string.Empty) && !serverPath.Equals(string.Empty))
            {
                return Path.Combine(serverPath, path);
            }

            return string.Empty;
        }
        #endregion
    }

}