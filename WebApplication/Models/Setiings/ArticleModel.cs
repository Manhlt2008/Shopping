using System.Web.Mvc;
using Microsoft.SqlServer.Server;

namespace WebApplication.Models.Setiings
{
    public class ArticleModel
    {
        public long Type { get; set; }

        public string TypeName { get; set; }

        public string Title { get; set; }

        [AllowHtml]
        public string Content { get; set; }


    }
}