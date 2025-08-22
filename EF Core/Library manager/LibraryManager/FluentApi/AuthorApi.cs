using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AuthorApi : IEntityTypeConfiguration<Author> {
    public void Configure(EntityTypeBuilder<Author> builder) {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(StringLength.AuthorName);
        builder.Property(a => a.IsAlive)
            .IsRequired();
    }
}