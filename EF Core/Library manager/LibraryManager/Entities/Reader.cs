public class Reader 
{
    public int Id { get; set; }
    public string FavoriteAuthor { get; set; }  // Название любимого автора (строка)
    public int? AuthorId { get; set; }         // Внешний ключ к Author
    public int BooksPurchased { get; set; }
    
    public Author Author { get; set; }       
}