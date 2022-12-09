
namespace GettingMessagesTelegram.Data
{
    public class MessageTranslate
    {
        /// <summary>
        /// Id translate
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Id message
        /// </summary>
        public long MessageId { get; set; }

        /// <summary>
        /// Translated content of message
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// DateCreated translate
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Language of translate
        /// </summary>
        public string Language { get; set; }

    /// <summary>
    /// Date published message
    /// </summary>
    public DateTime? PublishData { get; set; }

        public virtual Message Message { get; set; }
    }
}
