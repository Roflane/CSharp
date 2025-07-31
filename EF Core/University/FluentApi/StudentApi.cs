using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StudentApi : IEntityTypeConfiguration<Student> {
    public void Configure(EntityTypeBuilder<Student> builder) {
        builder.HasKey(s => s.Id);  
        builder
            .Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(254);
    }
}