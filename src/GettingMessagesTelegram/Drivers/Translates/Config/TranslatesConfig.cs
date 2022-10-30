
namespace GettingMessagesTelegram.Drivers.Translates.Config
{
    /// <summary>
    /// Configurations fro translating
    /// </summary>
    public class TranslatesConfig
    {
        /// <summary>
        /// Source language for messages and comments
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Destinations languages
        /// </summary>
        public List<string> Dests { get; set; }
    }
}
