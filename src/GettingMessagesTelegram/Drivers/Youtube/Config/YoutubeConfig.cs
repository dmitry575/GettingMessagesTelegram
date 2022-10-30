using Microsoft.Extensions.Configuration;

namespace GettingMessagesTelegram.Drivers.Youtube.Config
{
    /// <summary>
    /// Configuration of youtube
    /// </summary>
    public class YoutubeConfig
    {
        [ConfigurationKeyName("client_id")]
        public string ClientId { get; set; }

        [ConfigurationKeyName("client_secret")]
        public string ClientSecret { get; set; }

        [ConfigurationKeyName("redirect_uris")]
        public List<string> RedirectUris { get; set; }

        [ConfigurationKeyName("auth_uri")]
        public string AuthUri { get; set; }

        [ConfigurationKeyName("token_uri")]
        public string TokenUri { get; set; }
    }
}
