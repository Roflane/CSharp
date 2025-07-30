using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StudentCourseApi : IEntityTypeConfiguration<StudentCourse> {
    public void Configure(EntityTypeBuilder<StudentCourse> builder) {
        builder.HasKey(sc => new { sc.CourseId, sc.StudentId });
        
        builder
            .HasOne(sc => sc.Course)
            .WithMany(c => c.Courses)
            .HasForeignKey(sc => sc.CourseId);
        builder
            .HasOne(s => s.Student)
            .WithMany(s => s.Students)
            .HasForeignKey(sc => sc.StudentId);
    }
}