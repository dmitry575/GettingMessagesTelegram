using GettingMessagesTelegram.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GettingMessagesTelegram.DataAccess;

public class ChannelMap : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        builder.ToTable("Channels");
        builder.HasKey(p => p.Id);
    }
}
