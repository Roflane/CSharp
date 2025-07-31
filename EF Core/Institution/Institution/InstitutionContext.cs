using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class InstitutionContext : DbContext {
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentProfile> StudentProfiles { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<CourseAssignment> CourseAssignments { get; set; }
    public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<ExamResult> ExamResults { get; set; }
    
     protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Profile)
                .WithOne(p => p.Student)
                .HasForeignKey<StudentProfile>(p => p.StudentId);
            
            modelBuilder.Entity<Enrollment>()
                .HasKey(e => e.Id);
                
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId);
                
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);
            
            modelBuilder.Entity<CourseAssignment>()
                .HasKey(ca => new { ca.InstructorId, ca.CourseId });
                
            modelBuilder.Entity<CourseAssignment>()
                .HasOne(ca => ca.Instructor)
                .WithMany(i => i.CourseAssignments)
                .HasForeignKey(ca => ca.InstructorId);
                
            modelBuilder.Entity<CourseAssignment>()
                .HasOne(ca => ca.Course)
                .WithMany(c => c.CourseAssignments)
                .HasForeignKey(ca => ca.CourseId);
            
            modelBuilder.Entity<Instructor>()
                .HasOne(i => i.OfficeAssignment)
                .WithOne(o => o.Instructor)
                .HasForeignKey<OfficeAssignment>(o => o.InstructorId);
            
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.DepartmentId);
            
            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Exams)
                .HasForeignKey(e => e.CourseId);
            
            modelBuilder.Entity<ExamResult>()
                .HasOne(er => er.Exam)
                .WithMany(e => e.ExamResults)
                .HasForeignKey(er => er.ExamId);
                
            modelBuilder.Entity<ExamResult>()
                .HasOne(er => er.Student)
                .WithMany(s => s.ExamResults)
                .HasForeignKey(er => er.StudentId);
            
            modelBuilder.Entity<Instructor>()
                .HasOne(i => i.Department)
                .WithMany(d => d.Instructors)
                .HasForeignKey(i => i.DepartmentId);
     }

     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
         var connectionString = new ConfigurationBuilder()
             .AddJsonFile("sqlsettings.json")
             .Build()
             .GetConnectionString("Default");
        
         optionsBuilder.UseSqlServer(connectionString);         
     }
}