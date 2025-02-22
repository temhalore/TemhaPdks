namespace LorePdks.COMMON.Helpers
{
    public class ImageUrlObject
    {
        public string Base64ImageUrl { get; set; } = "";

        public string ImageUrl { get; set; } = "";
        public string UploadUrl { get; set; } = "";
    }
    public class ImageHelper
    {
        private static string _appKey = "678EFB0041F34574B9C9A196A9DD936E";

        public static ImageUrlObject GetImageUrlObject(string IUID, int imageType)
        {
            if (string.IsNullOrEmpty(IUID))
                return new ImageUrlObject();

            //var client = new CryptoService.CryptoServiceSoapClient();
            //var EID = client.Encrypt("userName", "password", IUID);
            var EID = CryptoHelper.EncryptFotoString(IUID);
            //var EID = IUID;

            return new ImageUrlObject()
            {
                Base64ImageUrl = GetImageAsBase64Url(string.Format("https://amficdn.istanbul.edu.tr/file/{0}/{1}?t={2}", _appKey, EID, imageType)),
                ImageUrl = string.Format("https://amficdn.istanbul.edu.tr/file/{0}/{1}?t={2}", _appKey, EID, imageType),
                UploadUrl = string.Format("https://amficdn.istanbul.edu.tr/Photo.aspx?app={0}&i={1}&t={2}", _appKey, EID, imageType)
            };
        }

        public static string GetImageAsBase64Url(string url)
        {

            using (var handler = new HttpClientHandler { })
            using (var client = new HttpClient(handler))
            {
                var bytes = client.GetByteArrayAsync(url).GetAwaiter().GetResult();

                //return "data:image/jpeg+jpg+png;base64," + Convert.ToBase64String(bytes);
                List<string> signatures = new List<string>{
                        "JVBERi0_pdf",
                        "R0lGODdh_gif",
                        "R0lGODlh_gif",
                        "iVBORw0KGgo_png",
                        "/9j/_jpg"
                        };

                string mime = "*";

                foreach (var s in signatures)
                {
                    string[] sign = s.Split('_');
                    if (Convert.ToBase64String(bytes).StartsWith(sign[0]))
                    {
                        mime = sign[1];
                    }
                }

                return string.Format("data:image/{0};base64,", mime) + Convert.ToBase64String(bytes);
            }
        }

    }

}
