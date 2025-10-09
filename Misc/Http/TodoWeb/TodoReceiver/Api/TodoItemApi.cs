using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoListWebApplication.Models;

namespace TodoReceiver.Api;

public class ToDoItemApi : IEntityTypeConfiguration<ToDoItem> {
    public void Configure(EntityTypeBuilder<ToDoItem> builder) {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(t => t.Description)
            .HasMaxLength(1000);
        
        builder.HasMany(t => t.Tags)
            .WithMany(tag => tag.Items)
            .UsingEntity(j => j.ToTable("TodoItemTags"));
    }
}