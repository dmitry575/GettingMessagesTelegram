
using TL;

namespace GettingMessagesTelegram.Media
{
    /// <summary>
    /// Create media object from any MessageMedia
    /// </summary>
    public interface IMediaCreator
    {
        Data.Media Create(long messageId, MessageMedia media);
    }
}
