using Microsoft.Extensions.Configuration;

namespace GettingMessagesTelegram.Drivers.Youtube.Config
{
    /// <summary>
    /// Configuration of youtube
    /// </summary>
    public class YoutubeConfig
    {
        /// <summary>
        /// Client id for outh2 authorize
        /// </summary>
        [ConfigurationKeyName("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Secret string for outh2 authorize
        /// </summary>
        [ConfigurationKeyName("client_secret")]
        public string ClientSecret { get; set; }

        [ConfigurationKeyName("redirect_uris")]
        public List<string> RedirectUris { get; set; }

        [ConfigurationKeyName("auth_uri")]
        public string AuthUri { get; set; }

        [ConfigurationKeyName("token_uri")]
        public string TokenUri { get; set; }

        /// <summary>
        /// Default title for videos
        /// </summary>
        [ConfigurationKeyName("default_title")]
        public string DefaultTitle { get; set; }

        /// <summary>
        /// Url for links display youtube videos
        /// </summary>
        [ConfigurationKeyName("youtube_url")]
        public string YoutubeUrl { get; set; }
        
    }
}
