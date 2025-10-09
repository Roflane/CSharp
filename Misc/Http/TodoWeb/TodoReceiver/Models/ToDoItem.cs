namespace ToDoListWebApplication.Models;

public class ToDoItem {
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}