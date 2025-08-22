using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Library;

public class LibraryContext : DbContext {
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Release> Releases { get; set; }
    public DbSet<Reader> Readers { get; set; }

    public XLogger logger;
    
    public LibraryContext() {}
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        logger = new("logs.txt");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}