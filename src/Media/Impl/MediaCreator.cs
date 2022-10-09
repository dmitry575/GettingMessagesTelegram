
using GettingMessagesTelegram.Enums;
using TL;

namespace GettingMessagesTelegram.Media.Impl
{
    public class MediaCreator : IMediaCreator
    {
        public DataAccess.Media Create(long messageId, MessageMedia media)
        {
            switch (media)
            {
                case MessageMediaWebPage webPage:

                    if (webPage.webpage is WebPage)
                    {
                        var w = webPage.webpage as WebPage;
                        return new DataAccess.Media
                        {
                            BaseId = w.ID,
                            Description = w.description,
                            HashAccess = w.hash,
                            MediaType = MediaType.WebPage,
                            Url = w.url,
                            MessageId = messageId
                        };
                    }
                    return null;

                    case MessageMediaDocument document:
                    if (document.document is TL.Document)
                    {
                        var d = document.document as Document;
                        return new DataAccess.Media
                        {
                            BaseId = d.ID,
                            Description = w.description,
                            HashAccess = d.access_hash,
                            MediaType = MediaType.Video,
                            Url = w.url,
                            MessageId = messageId
                        }

                   }
            }
                    return null;
        }
    }
}
