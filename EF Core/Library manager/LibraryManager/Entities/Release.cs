public class Release {
    public int Id { get; set; }
    public int AuthorId { get; set; }    
    public string Title { get; set; }   
    
    public Author Author { get; set; }
}