
using TL;

namespace GettingMessagesTelegram.Media
{
    /// <summary>
    /// Create media object from any MessageMedia
    /// </summary>
    public interface IMediaCreator
    {
        DataAccess.Media Create(long messageId, MessageMedia media);
    }
}
