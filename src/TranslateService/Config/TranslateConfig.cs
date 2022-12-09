namespace TranslateService.Config
{
    public class TranslateConfig
    {
        /// <summary>
        /// Url for translating service
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Destinations languages
        /// </summary>
        public List<string> DestLanguages { get; set; }

        /// <summary>
        /// The source language of message
        /// </summary>
        public string SourceLanguage { get; set; }
    }
}
