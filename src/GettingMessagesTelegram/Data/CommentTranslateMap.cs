using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GettingMessagesTelegram.Data;

internal class CommentTranslateMap : IEntityTypeConfiguration<CommentTranslate>
{
    public void Configure(EntityTypeBuilder<CommentTranslate> builder)
    {
        builder.ToTable("CommentsTranslates");
        builder.HasKey(p => p.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        builder.Property(b => b.DateCreated)
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAdd();

        builder.HasOne(p => p.Comment)
            .WithMany(p => p.Translates)
            .HasForeignKey(x => x.CommentId);
    }
}