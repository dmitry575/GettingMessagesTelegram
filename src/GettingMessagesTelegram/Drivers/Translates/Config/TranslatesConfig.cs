
namespace GettingMessagesTelegram.Drivers.Translates.Config
{
    /// <summary>
    /// Configurations fro translating
    /// </summary>
    public class TranslatesConfig
    {
        public const string FormatSeparate = "---{0}---";

        public const string FindSeparate = "---([0-9]+)---";

        /// <summary>
        /// Source language for messages and comments
        /// </summary>
        public string SourceLanguage { get; set; }

        /// <summary>
        /// Destinations languages
        /// </summary>
        public List<string> DestLanguages { get; set; }
        
        /// <summary>
        /// Path where will be saved messages of <Source> language
        /// </summary>
        public string SourcePath { get; set; }
        
        /// <summary>
        /// Path where save all translates for
        /// </summary>
        public string DestPath { get; set; }
    }
}
