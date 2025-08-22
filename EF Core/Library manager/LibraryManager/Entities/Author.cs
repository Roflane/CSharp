public class Author {
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsAlive { get; set; }
    
    public ICollection<Reader> Readers { get; set; }
    public ICollection<Release> Releases { get; set; }
}