using GettingMessagesTelegram.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GettingMessagesTelegram.DataAccess;

public class CommentMap:IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");
        
        builder.HasKey(p => p.Id);

        builder.HasOne<Message>(p => p.Message)
            .WithMany(p => p.Comments)
            .HasForeignKey(x => x.MessageId);
    }
}
