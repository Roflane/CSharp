using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ToDoListWebApplication.Models;

namespace ToDoListWebApplication.Todo {
    public class ToDoContext : DbContext
    {
        public ToDoContext(DbContextOptions<ToDoContext> options)
            : base(options) { }

        public DbSet<ToDoItem> ToDoItems { get; set; }
    }

}