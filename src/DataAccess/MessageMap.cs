using GettingMessagesTelegram.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GettingMessagesTelegram.DataAccess;

public class MessageMap : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasOne<Channel>(p => p.Channel)
            .WithMany(p => p.Messages)
            .HasForeignKey(x => x.ChannelId);
    }
}
