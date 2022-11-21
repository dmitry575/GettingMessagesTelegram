using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GettingMessagesTelegram.Data;
using GettingMessagesTelegram.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace GettingMessagesTelegram.Services.Impl;

public class CommentsService : ICommentsService
{
    private readonly MessagesContext _messagesContext;

    public CommentsService(MessagesContext messagesContext)
    {
        _messagesContext = messagesContext;
    }

    public async Task<List<Comment>> GetNotTranslate(string language, int page, int countRows)
    {
        return await _messagesContext
            .Comments
            .AsQueryable()
            .AsNoTracking()
            .Include(x => x.Translates)
            .Include(x => x.Message)
            .Include(x => x.Message.Translates)
            .Where(x => x.Message.Translates.Any() && x.Message.Translates.Any(t => t.Language == language))
            .Where(x => !x.Translates.Any() || x.Translates.All(t => t.Language != language))
            .OrderBy(x => x.MessageId)
            .Skip(page * countRows)
            .Take(countRows)
            .ToListAsync();
    }

    public async Task<List<Comment>> GetNotTranslate(string language, long messageId)
    {
        return await _messagesContext
            .Comments
            .AsNoTracking()
            .Include(x => x.Translates)
            .Where(x => x.MessageId == messageId)
            .Where(x => !x.Translates.Any() || x.Translates.All(t => t.Language != language))
            .ToListAsync();
    }
}

