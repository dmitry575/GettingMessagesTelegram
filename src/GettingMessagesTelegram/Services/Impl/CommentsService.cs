using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GettingMessagesTelegram.Data;

namespace GettingMessagesTelegram.Services.Impl;

    public class CommentsService: ICommentsService
    {
        public Task<List<Comment>> GetNotTranslate(string language, int page, int countRows)
        {
            throw new NotImplementedException();
        }

        public Task<List<Comment>> GetNotTranslate(string language, long messageId)
        {
            throw new NotImplementedException();
        }
    }

