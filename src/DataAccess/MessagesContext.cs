using GettingMessagesTelegram.Data;
using Microsoft.EntityFrameworkCore;

namespace GettingMessagesTelegram.DataAccess;

public class MessagesContext : DbContext
{
    public DbSet<Channel> Channel { get; set; }

    public DbSet<Message> Messages { get; set; }

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
            .ConfigureDateTimeToUtc();
    }

    public MessagesContext(string connectionString)
    {
        _connectionString = connectionString;
    }
}
