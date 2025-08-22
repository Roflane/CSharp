public class Reader {
    public int Id { get; set; }
    public string FavoriteAuthor { get; set; } 
    public int? AuthorId { get; set; }      
    public int BooksPurchased { get; set; }
    
    public Author Author { get; set; }       
}