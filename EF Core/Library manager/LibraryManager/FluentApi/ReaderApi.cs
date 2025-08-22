using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ReaderApi : IEntityTypeConfiguration<Reader> 
{
    public void Configure(EntityTypeBuilder<Reader> builder) 
    {
        builder.HasKey(r => r.Id);
        
        builder.HasOne(r => r.Author)
            .WithMany(a => a.Readers)
            .HasForeignKey(r => r.AuthorId)  
            .IsRequired(false); 
        
        builder.Property(r => r.BooksPurchased)
            .IsRequired();
        
        builder.Property(r => r.FavoriteAuthor)
            .HasMaxLength(100);
    }
}