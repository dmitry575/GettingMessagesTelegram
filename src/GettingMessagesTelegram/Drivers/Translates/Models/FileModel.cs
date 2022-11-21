namespace GettingMessagesTelegram.Drivers.Translates.NewFolder
{
    /// <summary>
    /// Model of file 
    /// </summary>
    public class FileModel
    {
        /// <summary>
        /// Language fo translated
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Id of message
        /// </summary>
        public long MessageId{ get; set; }

        /// <summary>
        /// How many comments will be in file
        /// </summary>
        public long CountComments { get; set; }
    }
}
