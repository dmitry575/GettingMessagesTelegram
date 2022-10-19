using GettingMessagesTelegram.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GettingMessagesTelegram.DataAccess;

public class MediaMap: IEntityTypeConfiguration<Data.Media>
{
    public void Configure(EntityTypeBuilder<Data.Media> builder)
    {
        builder.ToTable("Medias");
        builder.HasKey(p => p.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        
        builder.HasOne<Message>(p => p.Message)
            .WithMany(p => p.Medias)
            .HasForeignKey(x => x.MessageId);
    }
}
