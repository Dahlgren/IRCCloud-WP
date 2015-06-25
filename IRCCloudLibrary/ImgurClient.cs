using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IRCCloudLibrary
{
    public class ImgurImage
    {
        public string ID { get; set; }
        [JsonProperty(PropertyName = "deletehash")]
        public string DeleteHash { get; set; }
        public string Title { get; set; }
        public Int64 DateTime { get; set; }
        public string Type { get; set; }
        [JsonProperty(PropertyName = "animated")]
        public bool IsAnimated { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Int64 Size { get; set; }
        public Int64 Views { get; set; }
        [JsonProperty(PropertyName = "account_url")]
        public string AccountUrl { get; set; }
        public string Link { get; set; }
        public string Bandwidth { get; set; }
        public int Ups { get; set; }
        public int Downs { get; set; }
        public int Score { get; set; }
        [JsonProperty(PropertyName = "is_album")]
        public bool IsAlbum { get; set; }
    }

    public class ImgurImageUpload
    {
        [JsonProperty(PropertyName = "data")]
        public ImgurImage Image { get; set; }
        public bool Success { get; set; }
        public int Status { get; set; }
    }

    public class ImgurClient
    {
        private string ClientId  { get; set; }

        public ImgurClient(string clientId)
        {
            ClientId = clientId;
        }

        public async Task<ImgurImageUpload> UploadImage(byte[] imageData)
        {
            StringBuilder builder = new StringBuilder();
            string base64imageString = Convert.ToBase64String(imageData);

            var json = new JObject();
            json["image"] = base64imageString;

            var httpClient = new HttpClient();
            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Add("Authorization", "Client-ID " + ClientId);

            var response = await httpClient.PostAsync("https://api.imgur.com/3/upload", httpContent);

            return JsonConvert.DeserializeObject<ImgurImageUpload>(await response.Content.ReadAsStringAsync());
        }
    }
}
