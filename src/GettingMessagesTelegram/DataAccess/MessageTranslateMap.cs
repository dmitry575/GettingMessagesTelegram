using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using GettingMessagesTelegram.Data;

namespace GettingMessagesTelegram.DataAccess;

public class MessageTranslateMap : IEntityTypeConfiguration<MessageTranslate>
{
    public void Configure(EntityTypeBuilder<MessageTranslate> builder)
    {
        builder.ToTable("MessagesTranslates");
        builder.HasKey(p => p.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        builder.Property(b => b.DateCreated)
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAdd();

        builder.HasOne(p => p.Message)
            .WithMany(p => p.Translates)
            .HasForeignKey(x => x.MessageId);
    }
}