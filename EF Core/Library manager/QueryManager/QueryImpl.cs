using System.ComponentModel;
using System.Windows;
using LibraryManager.Library;

namespace LibraryManager.QueryManager;

public class QueryImpl : INotifyPropertyChanged {
    private readonly LibraryContext _dbContext;
    private string _searchText;
    
    private List<Author> _authors;
    private List<Author> _filteredAuthors;
    private List<Book> _books;
    private List<Book> _filteredBooks;
    private List<Reader> _readers;
    private List<Reader> _filteredReaders;
    private List<Release> _releases;
    private List<Release> _filteredReleases;
    
    public QueryImpl(LibraryContext dbContext) {
        _dbContext = dbContext;
        _books = new List<Book>();
        _filteredBooks = new List<Book>();
        _authors = new List<Author>();
        _filteredAuthors = new List<Author>();
        _readers = new List<Reader>();
        _filteredReaders = new List<Reader>();
        _releases = new List<Release>();
        _filteredReleases = new List<Release>();
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    /// <summary>
    /// <para> Usage: read [Table] </para>
    /// Example: read Books
    /// </summary>
    private void HandleQueryRead() {
        if (_searchText.Trim().Contains(QuerySyntax.R)) {
            try {
                string tableName = _searchText.Replace(QuerySyntax.R, string.Empty);
                
                switch (tableName) {
                    case "Authors":
                        FilteredAuthors = _authors;
                        break;
                    case "Books":
                        FilteredBooks = _books;
                        break;
                    case "Readers":
                        FilteredReaders = _readers;
                        break;
                    case "Releases":
                        FilteredReleases = _releases;
                        break;
                } 

                MessageBox.Show(QueryLog.SuccessRead);
                _dbContext.logger.LogToFile($"{tableName} | {QueryLog.SuccessRead}");
            }
            catch (Exception ex) {
                string error = $"{QueryLog.ErrorRead} {ex.Message}";
                _dbContext.logger.LogToFile(error);
                MessageBox.Show(error);
            }
        }
    }
    
    /// <summary>
    /// <para> Usage: create value in [Table] (Var name = value, ...) </para>
    /// Example: create value in Books (Title = A, ReleaseDate = '05-05-2030') 
    /// </summary>
    private void HandleQueryCreate() {
        if (_searchText.Trim().StartsWith(QuerySyntax.C)) {
            try {
                string[] parts = _searchText.Split(["(", ")"], StringSplitOptions.RemoveEmptyEntries);
                string tableName = parts[0].Replace(QuerySyntax.C, "").Trim();
                string[] parameters = parts[1].Split(',');

                Dictionary<string, string> props = new();
                foreach (var param in parameters) {
                    string[] keyValue = param.Split('=');
                    props[keyValue[0].Trim()] = keyValue[1].Trim().Trim('\'');
                }
                
                switch (tableName) {
                    case "Authors":
                        var author = new Author {
                            Name = props["Name"],
                            IsAlive = bool.Parse(props["IsAlive"])
                        };
                        _dbContext.Authors.Add(author);
                        break;
                    
                    case "Books":
                        var book = new Book {
                            Title = props["Title"],
                            ReleaseDate = DateTime.Parse(props["ReleaseDate"])
                        };
                        _dbContext.Books.Add(book);
                        break;
                    case "Readers":
                        var reader = new Reader {
                            FavoriteAuthor = props["FavoriteAuthor"],
                            AuthorId = Convert.ToInt32(props["AuthorId"])
                        };
                        _dbContext.Readers.Add(reader);
                        break;
                    case "Releases":
                        var release = new Release {
                            AuthorId = Convert.ToInt32(props["AuthorId"]),
                            Title = props["Title"]
                        };
                        _dbContext.Releases.Add(release);
                        break;
                }

                if (_dbContext.SaveChanges() > 0) {
                    MessageBox.Show(QueryLog.SuccessCreate);
                    _dbContext.logger.LogToFile($"{tableName} | {QueryLog.SuccessCreate}");
                }
            }
            catch (Exception ex) {
                string error = $"{QueryLog.ErrorCreate} {ex.Message}";
                _dbContext.logger.LogToFile(error);
                MessageBox.Show(error);
            }
        }
    }
    
    /// <summary>
    /// <para> Usage: update value in [T, Id] (Var name = value, ...) </para>
    /// Example: update value in Books, 1 (Title = Modified, ReleaseDate = 03-03-2023)
    /// </summary>

    private void HandleQueryUpdate() {
        if (_searchText.Trim().StartsWith(QuerySyntax.U)) {
            try {
                string[] parts = _searchText.Split(["(", ")"], StringSplitOptions.RemoveEmptyEntries);
                string tablePart = parts[0].Replace(QuerySyntax.U, "").Trim();
                string[] tableAndId = tablePart.Split(',');
                string tableName = tableAndId[0].Trim();
                int id = Convert.ToInt32(tableAndId[1].Trim());

                string[] parameters = parts[1].Split(',');
                Dictionary<string, string> props = new();
                foreach (var param in parameters) {
                    int equalSignIndex = param.IndexOf('=');
                    if (equalSignIndex > 0) {
                        string key = param.Substring(0, equalSignIndex).Trim();
                        string value = param.Substring(equalSignIndex + 1).Trim().Trim('\'', '"');
                        props[key] = value;
                    }
                }

                switch (tableName) {
                    case "Authors":
                        var author = _dbContext.Authors.FirstOrDefault(a => a.Id == id);
                        if (author != null) {
                            author.Name = props["Name"];
                            author.IsAlive = bool.Parse(props["IsAlive"]);
                        }
                        break;
                    case "Books":
                        var book = _dbContext.Books.FirstOrDefault(b => b.Id == id);
                        if (book != null) {
                            book.Title = props["Title"];
                            book.ReleaseDate = DateTime.Parse(props["ReleaseDate"]);
                        }
                        break;
                    case "Readers":
                        var reader = _dbContext.Readers.FirstOrDefault(r => r.Id == id);
                        if (reader != null) {
                            reader.FavoriteAuthor = props["FavoriteAuthor"];
                            reader.AuthorId = Convert.ToInt32(props["AuthorId"]);
                        }
                        break;
                    case "Releases":
                        var release = _dbContext.Releases.FirstOrDefault(r => r.Id == id);
                        if (release != null) {
                            release.Title = props["Title"];
                            release.AuthorId = Convert.ToInt32(props["AuthorId"]);
                        }
                        break;
                }

                if (_dbContext.SaveChanges() > 0) {
                    MessageBox.Show(QueryLog.SuccessUpdate);
                    _dbContext.logger.LogToFile($"{tableName} | {QueryLog.SuccessUpdate}");
                }
            }
            catch (Exception ex) {
                string error = $"{QueryLog.ErrorUpdate} {ex.Message}";
                MessageBox.Show(error);
                _dbContext.logger.LogToFile(error);
            }
        }
    }
    
    /// <summary>
    /// <para> Usage: delete value in [T, Id] </para>
    /// Example: delete value in [Books, 1]
    /// </summary>
    private void HandleQueryDelete() {
        if (_searchText.Trim().StartsWith(QuerySyntax.D)) {
            try {
                string content = _searchText.Replace(QuerySyntax.D, "").Trim();
                string insideBrackets = content.Trim('[', ']');
                
                string[] tableAndId = insideBrackets.Split(',');
                string tableName = tableAndId[0].Trim();
                int id = Convert.ToInt32(tableAndId[1].Trim());
                
                switch (tableName) {
                    case "Authors":
                        var author = _dbContext.Authors.FirstOrDefault(a => a.Id == id);
                        if (author != null) _dbContext.Authors.Remove(author);
                        break;
                    case "Books":
                        var book = _dbContext.Books.FirstOrDefault(b => b.Id == id);
                        if (book != null) _dbContext.Books.Remove(book);
                        break;
                    case "Readers":
                        var reader = _dbContext.Readers.FirstOrDefault(r => r.Id == id);
                        if (reader != null) _dbContext.Readers.Remove(reader);
                        break;
                    case "Releases":
                        var release = _dbContext.Releases.FirstOrDefault(r => r.Id == id);
                        if (release != null) _dbContext.Releases.Remove(release);
                        break;
                }

                if (_dbContext.SaveChanges() > 0) {
                    MessageBox.Show(QueryLog.SuccessDelete);
                    _dbContext.logger.LogToFile($"{tableName} | {QueryLog.SuccessDelete}");
                }
            }
            catch (Exception ex) {
                string error = $"{QueryLog.ErrorCreate} {ex.Message}";
                _dbContext.logger.LogToFile(error);
                MessageBox.Show(error);
            }
        }
    }
    
    public void ProcessSqlQuery() {
        HandleQueryCreate();
        HandleQueryRead();
        HandleQueryUpdate();
        HandleQueryDelete();
    }
    
    public string SearchText {
        get => _searchText;
        set => _searchText = value;
    }
    
    // Books
    public List<Book> FilteredBooks {
        get => _filteredBooks;
        private set {
            _filteredBooks = value;
            OnPropertyChanged(nameof(FilteredBooks));
        }
    }
    
    public void LoadBooks() {
        _books = _dbContext.Books.ToList();
        FilteredBooks = new List<Book>();
    }

    public void FilterBooks() {
        if (string.IsNullOrWhiteSpace(_searchText)) {
            FilteredBooks = new List<Book>();
        }
    }
    
    // Authors
    public List<Author> FilteredAuthors {
        get => _filteredAuthors;
        private set {
            _filteredAuthors = value;
            OnPropertyChanged(nameof(FilteredAuthors));
        }
    }
    
    public void LoadAuthors() {
        _authors = _dbContext.Authors.ToList();
        FilteredAuthors = new List<Author>();
    }
    
    public void FilterAuthors() {
        if (string.IsNullOrWhiteSpace(_searchText)) {
            FilteredAuthors = new List<Author>();
        }
    }
    
    // Readers
    public List<Reader> FilteredReaders {
        get => _filteredReaders;
        private set {
            _filteredReaders = value;
            OnPropertyChanged(nameof(FilteredReaders));
        }
    }
    
    public void LoadReaders() {
        _readers = _dbContext.Readers.ToList();
        FilteredReaders = new List<Reader>();
    }
    
    public void FilterReaders() {
        if (string.IsNullOrWhiteSpace(_searchText)) {
            FilteredReaders = new List<Reader>();
        }
    }
    
    // Releases
    public List<Release> FilteredReleases {
        get => _filteredReleases;
        private set {
            _filteredReleases = value;
            OnPropertyChanged(nameof(FilteredReleases));
        }
    }
    
    public void LoadReleases() {
        _releases = _dbContext.Releases.ToList();
        FilteredReleases = new List<Release>();
    }
    
    public void FilterReleases() {
        if (string.IsNullOrWhiteSpace(_searchText)) {
            FilteredReleases = new List<Release>();
        }
    }
}