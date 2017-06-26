using System.IO;
using System.Web.Mvc;

namespace WebApplication.Common
{
    public static class ControllerExtensions
    {

        public static ImageResult Image(this Controller controller, Stream imageStream, string contentType)
        {
            return new ImageResult(imageStream, contentType);
        }

        public static ImageResult Image(this Controller controller, byte[] imageBytes, string contentType)
        {
            try
            {
                return new ImageResult(new MemoryStream(imageBytes), contentType);
            }
            catch
            {
                return new ImageResult(new MemoryStream(), contentType);

            }
        }
    }
}