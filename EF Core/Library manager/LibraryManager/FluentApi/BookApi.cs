using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BookApi : IEntityTypeConfiguration<Book> {
    public void Configure(EntityTypeBuilder<Book> builder) {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(StringLength.BookTitle);
        builder.Property(b => b.ReleaseDate)
            .IsRequired();
    }
}