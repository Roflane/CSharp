using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LibraryManager.Library;

public class LibraryContext : DbContext {
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Release> Releases { get; set; }
    public DbSet<Reader> Readers { get; set; }
    
    public LibraryContext() {}
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        // var loggerFactory = new LoggerFactory();
        // loggerFactory.AddProvider(new XLogger());
        // optionsBuilder.UseLoggerFactory(loggerFactory);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}