using System.Web.Mvc;

namespace WebApplication.Models.Setiings
{
    public class FooterModel
    {
        [AllowHtml]
        public string About { get; set; }

        [AllowHtml]
        public string Copyright { get; set; }

        public string FacebookUrl { get; set; }

        public string FacebookTabs { get; set; }

        public string FacebookHeight { get; set; }

        public string FacebookWidth { get; set; }

        public string FacebookTitle { get; set; }

        public string FollowUsFacebook { get; set; }
        public string FollowUsTwitter { get; set; }
        public string FollowUsGooglePlus { get; set; }
        public string FollowUsYouTube { get; set; }
    }
}