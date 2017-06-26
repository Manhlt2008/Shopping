using Newtonsoft.Json;

namespace WebApplication.Models.APIModel.DealToDay
{
    public class DealToDayObjectResultImage
    {
        [JsonProperty("imgGalleryId")]
        public int imgGalleryId { get; set; }

        [JsonProperty("imgGallerySrc")]
        public string imgGallerySrc { get; set; }
    }
}