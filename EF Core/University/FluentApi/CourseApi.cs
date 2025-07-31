using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CourseApi : IEntityTypeConfiguration<Course> {
    public void Configure(EntityTypeBuilder<Course> builder) {
        builder.HasKey(c => c.Id);
    }
}