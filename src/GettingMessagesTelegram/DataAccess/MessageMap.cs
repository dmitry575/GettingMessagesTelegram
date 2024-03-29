﻿using GettingMessagesTelegram.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GettingMessagesTelegram.DataAccess;

public class MessageMap : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");
        builder.HasKey(p => p.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        
        builder.HasOne<Channel>(p => p.Channel)
            .WithMany(p => p.Messages)
            .HasForeignKey(x => x.ChannelId);
    }
}
