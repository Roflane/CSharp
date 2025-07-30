using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class UniversityContext : DbContext {
    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<StudentCourse> StudentCourses { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        modelBuilder
            .Entity<Course>()
            .ToTable(c => c.HasCheckConstraint("Credit", "Credit > 0"));
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        var connectionString = new ConfigurationBuilder()
            .AddJsonFile("sqlsettings.json")
            .Build()
            .GetConnectionString("Default");
        
        optionsBuilder.UseSqlServer(connectionString);
    }
}