using Newtonsoft.Json;

namespace GettingMessagesTelegram.Drivers.PostImage.Models
{
    public class UploadRequest
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("upload_session")]
        public string UploadSession { get; set; }

        [JsonProperty("numfiles")] public int Numfiles { get; set; } = 1;

        [JsonProperty("optsize")]
        public int Optsize { get; set; } = 0;

        [JsonProperty("gallery")]
        public string Gallery { get; set; }

        [JsonProperty("expire")]
        public int Expire { get; set; } = 0;
        
        [JsonProperty("source")]
        public string Source { get; set; } = "computer";
        

    }
}
