using LibraryManager.Library;

public static class ConstraintError {
    public static readonly string Book = $"Length of book title cannot be more than {StringLength.BookTitle}!";
    public static readonly string Author = $"Length of author name cannot be more than {StringLength.AuthorName}!";
    public static readonly string Reader = $"Length of favorite author in reader cannot be more than {StringLength.ReaderFavoriteAuthor}!";
    public static readonly string Release = $"Length of release title in reader cannot be more than {StringLength.ReleaseTitle}!";
}

public class ConstraintManager {
    private LibraryContext _dbContext;
    private static int a;
    public ConstraintManager(LibraryContext dbContext) {
        _dbContext = dbContext;
    }
    
    public bool IsValidBook(Book book) {
        if (book.Title.Length > StringLength.ReleaseTitle) {
            return false;
        }
        return true;
    }

    public bool IsValidAuthor(Author author) {
        if (author.Name.Length > StringLength.AuthorName) {
            return false;
        }
        return true;
    }

    public bool IsValidReader(Reader reader) {
        if (reader.FavoriteAuthor.Length > StringLength.ReaderFavoriteAuthor) {
            return false;
        }
        return true;
    }

    public bool IsValidRelease(Release release) {
        if (release.Title.Length > StringLength.ReleaseTitle) {
            return false;
        }
        return true;
    }
}