namespace ToDoListWebApplication.Models;

public class Tag {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<ToDoItem> Items { get; set; } = new List<ToDoItem>();
}