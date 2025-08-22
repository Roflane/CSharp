using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ReleaseApi : IEntityTypeConfiguration<Release> {
    public void Configure(EntityTypeBuilder<Release> builder) {
        builder.HasKey(r => r.Id);
        builder.HasOne(r => r.Author)
            .WithMany(a => a.Releases)
            .HasForeignKey(r => r.AuthorId);
        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(StringLength.ReleaseTitle);
    }
}