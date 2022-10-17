using GettingMessagesTelegram.Data;
using Microsoft.EntityFrameworkCore;

namespace GettingMessagesTelegram.DataAccess;

public class MessagesContext : DbContext
{
    public DbSet<Channel> Channels { get; set; }

    public DbSet<Message> Messages { get; set; }
    
    public DbSet<Comment> Comments { get; set; }
    
    public DbSet<Data.Media> Medias { get; set; }

    public MessagesContext(DbContextOptions<MessagesContext> options)
        : base(options)
    {
         
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();

        optionsBuilder.LogTo(log => Console.WriteLine(log));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new ChannelMap())
            .ApplyConfiguration(new MessageMap())
            .ApplyConfiguration(new CommentMap())
            .ApplyConfiguration(new MediaMap())
            .ConfigureDateTimeToUtc();
    }
}
