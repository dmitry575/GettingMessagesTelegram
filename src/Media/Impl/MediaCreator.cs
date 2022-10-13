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
                        return CreateMedia(messageId, w);
                    }

                    return null;

                case MessageMediaDocument document:
                    if (document.document is Document)
                    {
                        var d = document.document as Document;
                        return CreateMedia(messageId, d);
                    }

                    return null;
                
                case MessageMediaPhoto photo:
                    if (photo.photo is Photo)
                    {
                        var p = photo.photo as Photo;
                        return CreateMedia(messageId, p);
                    }

                    return null;
            }

            return null;
        }

        private DataAccess.Media CreateMedia(long messageId, Photo photo)
            => new DataAccess.Media
            {
                BaseId = photo.ID,
                HashAccess = photo.access_hash,
                Type = MediaType.Photo,
                MimeType = photo.LargestPhotoSize.Type,
                MessageId = messageId,
                FileSize = photo.LargestPhotoSize.FileSize,
                FileName = photo.dc_id.ToString()
            };

        /// <summary>
        /// MEdia Video
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="document"></param>
        private DataAccess.Media CreateMedia(long messageId, Document document)
            => new DataAccess.Media
            {
                BaseId = document.ID,
                HashAccess = document.access_hash,
                Type = GetMediaType(document.mime_type),
                MimeType = document.mime_type,
                MessageId = messageId,
                FileSize = document.size,
                FileName = document.Filename
            };

        /// <summary>
        /// Create media from webpage
        /// </summary>
        /// <param name="messageId">Id message from table Messages</param>
        /// <param name="webPage">Information about page</param>
        private DataAccess.Media CreateMedia(long messageId, WebPage webPage)
            => new DataAccess.Media
            {
                BaseId = webPage.ID,
                Description = webPage.description,
                HashAccess = webPage.hash,
                Type = MediaType.WebPage,
                Url = webPage.url,
                MessageId = messageId
            };

        /// <summary>
        /// Get type of media by mime-Type
        /// </summary>
        /// <param name="mimeType"></param>
        private MediaType GetMediaType(string mimeType)
        {
            if (mimeType.StartsWith("video"))
            {
                return MediaType.Video;
            }

            if (mimeType.StartsWith("image"))
            {
                return MediaType.Photo;
            }

            return MediaType.None;
        }
    }
}
