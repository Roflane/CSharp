using System.ComponentModel;
using System.Windows;
using LibraryManager.Library;
using LibraryManager.Managers.DataManager;

namespace LibraryManager.QueryManager;


public class QueryImpl : INotifyPropertyChanged {
    private readonly LibraryContext _dbContext;
    private string _searchText;
    
    private ConstraintManager _constraintManager;
    public EntityDataManager<Author> AuthorData;
    public EntityDataManager<Book> BookData;
    public EntityDataManager<Reader> ReaderData;
    public EntityDataManager<Release> ReleaseData;
 
    public string SearchText {
        get => _searchText;
        set => _searchText = value;
    }
    
    public QueryImpl(LibraryContext dbContext) {
        _dbContext = dbContext;
        _constraintManager = new ConstraintManager(_dbContext);
        AuthorData = new(_dbContext, _searchText);
        BookData = new(_dbContext, _searchText);
        ReaderData = new(_dbContext, _searchText);
        ReleaseData = new(_dbContext, _searchText);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
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
                bool readFailed = false;
                string tableName = _searchText.Replace(QuerySyntax.R, string.Empty);
                
                switch (tableName) {
                    case "Authors":
                        AuthorData.LoadEntity();
                        break;
                    case "Books":
                        BookData.LoadEntity();
                        break;
                    case "Readers":
                        ReaderData.LoadEntity();
                        break;
                    case "Releases":
                        ReleaseData.LoadEntity();
                        break;
                    default:
                        readFailed = true;
                        break;
                }

                if (!readFailed) {
                    _dbContext.logger.LogToFile($"{tableName} | {QueryLog.SuccessRead}");
                    MessageBox.Show(QueryLog.SuccessRead, "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else {
                    _dbContext.logger.LogToFile($"{tableName} | {QueryLog.ErrorRead}");
                    MessageBox.Show(QueryLog.ErrorRead, "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
 
            }
            catch (Exception ex) {
                string error = $"{QueryLog.ErrorRead} {ex.Message}";
                _dbContext.logger.LogToFile(error);
                MessageBox.Show(error, "Error", MessageBoxButton.OK,  MessageBoxImage.Error);
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
                        
                        if (_constraintManager.IsValidAuthor(author)) {
                            _dbContext.Authors.Add(author);
                        }
                        else {
                            _dbContext.logger.LogToFile($"{tableName} | {QueryLog.ErrorCreate}, {ConstraintError.Author}");
                            MessageBox.Show(ConstraintError.Author, "Error", 
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        break;
                    case "Books":
                        var book = new Book {
                            Title = props["Title"],
                            ReleaseDate = DateTime.Parse(props["ReleaseDate"])
                        };

                        if (_constraintManager.IsValidBook(book)) {
                            _dbContext.Books.Add(book);
                        }
                        else {
                            _dbContext.logger.LogToFile($"{tableName} | {QueryLog.ErrorCreate}, {ConstraintError.Book}");
                            MessageBox.Show(ConstraintError.Book, "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        break;
                    case "Readers":
                        var reader = new Reader {
                            FavoriteAuthor = props["FavoriteAuthor"],
                            AuthorId = Convert.ToInt32(props["AuthorId"])
                        };
                        
                        if (_constraintManager.IsValidReader(reader)) {
                            _dbContext.Readers.Add(reader);
                        }
                        else {
                            _dbContext.logger.LogToFile($"{tableName} | {QueryLog.ErrorCreate}, {ConstraintError.Reader}");
                            MessageBox.Show(ConstraintError.Reader, "Error", 
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        break;
                    case "Releases":
                        var release = new Release {
                            AuthorId = Convert.ToInt32(props["AuthorId"]),
                            Title = props["Title"]
                        };
                        
                        if (_constraintManager.IsValidRelease(release)) {
                            _dbContext.Releases.Add(release);
                        }
                        else {
                            _dbContext.logger.LogToFile($"{tableName} | {QueryLog.ErrorCreate}, {ConstraintError.Release}");
                            MessageBox.Show(ConstraintError.Release, "Error", 
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        break;
                }

                if (_dbContext.SaveChanges() > 0) {
                    MessageBox.Show(QueryLog.SuccessCreate, "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    _dbContext.logger.LogToFile($"{tableName} | {QueryLog.SuccessCreate}");
                }
                else {
                    MessageBox.Show(QueryLog.ErrorCreate, "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    _dbContext.logger.LogToFile($"{tableName} | {QueryLog.ErrorCreate}");
                }
            }
            catch (Exception ex) {
                string error = $"{QueryLog.ErrorCreate} {ex.Message}";
                _dbContext.logger.LogToFile(error);
                MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show(QueryLog.SuccessUpdate, "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    _dbContext.logger.LogToFile($"{tableName} | {QueryLog.SuccessUpdate}");
                }
                else {
                    MessageBox.Show(QueryLog.ErrorUpdate, "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    _dbContext.logger.LogToFile($"{tableName} | {QueryLog.ErrorUpdate}");
                }
            }
            catch (Exception ex) {
                string error = $"{QueryLog.ErrorUpdate} {ex.Message}";
                _dbContext.logger.LogToFile(error);
                MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    _dbContext.logger.LogToFile($"{tableName} | {QueryLog.SuccessDelete}");
                    MessageBox.Show(QueryLog.SuccessDelete);
                } else {
                    _dbContext.logger.LogToFile($"{tableName} | {QueryLog.ErrorDelete}");
                    MessageBox.Show(QueryLog.ErrorDelete,  "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex) {
                string error = $"{QueryLog.ErrorCreate} {ex.Message}";
                _dbContext.logger.LogToFile(error);
                MessageBox.Show(error, "Error",  MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    
    public void ProcessSqlQuery() {
        HandleQueryCreate();
        HandleQueryRead();
        HandleQueryUpdate();
        HandleQueryDelete();
    }
}