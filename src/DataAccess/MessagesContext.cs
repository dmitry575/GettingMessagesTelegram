using GettingMessagesTelegram.Data;
using Microsoft.EntityFrameworkCore;

namespace GettingMessagesTelegram.DataAccess;

public class MessagesContext : DbContext
{
    public DbSet<Channel> Channels { get; set; }

    public DbSet<Message> Messages { get; set; }
    
    public DbSet<Comment> Comments { get; set; }

    private readonly string _connectionString;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();

        //optionsBuilder.LogTo(log => _logger.Debug(log));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new ChannelMap())
            .ApplyConfiguration(new MessageMap())
            .ApplyConfiguration(new CommentMap())
            .ConfigureDateTimeToUtc();
    }

    public MessagesContext(string connectionString)
    {
        _connectionString = connectionString;
    }
}
