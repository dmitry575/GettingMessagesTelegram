
namespace GettingMessagesTelegram.Data
{
    /// <summary>
    /// Translated of comment
    /// </summary>
    public class CommentTranslate
    {
        /// <summary>
        /// Id translate
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Id comment
        /// </summary>
        public long CommentId { get; set; }

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

        public virtual Comment Comment { get; set; }
    }
}
