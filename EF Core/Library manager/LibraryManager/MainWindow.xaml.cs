using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using LibraryManager.Library;
using LibraryManager.QueryManager;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LibraryManager;

public partial class MainWindow : Window, INotifyPropertyChanged {
    private readonly string _wndName = "LibraryManager";
    private QueryImpl _query;
    private string _searchText;
    
    public List<Book> FilteredBooks => _query.BookData.FilteredEntityList;
    public List<Author> FilteredAuthors => _query.AuthorData.FilteredEntityList;
    public List<Reader> FilteredReaders => _query.ReaderData.FilteredEntityList;
    public List<Release> FilteredReleases => _query.ReleaseData.FilteredEntityList;
    
    private void ConfigureSql() {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("sqlsettings.json")
            .Build();

        var options = new DbContextOptionsBuilder<LibraryContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"))
            .Options;
        
        
        LibraryContext libCtx = new(options);
        _query = new QueryImpl(libCtx);
        DataContext = this;
    }

    private void LoadInitData() {
        _query.AuthorData.LoadEntity();
        _query.BookData.LoadEntity();
        _query.ReaderData.LoadEntity();
        _query.ReleaseData.LoadEntity();
    }
    
    public MainWindow() {
        InitializeComponent();
        Title = _wndName;
        
        ConfigureSql();
        LoadInitData();
    }

    public string SearchText {
        get => _searchText;
        set {
            _searchText = value;
            _query.SearchText = value;
            _query.BookData.FilterEntities();
            _query.AuthorData.FilterEntities();
            _query.ReaderData.FilterEntities();
            _query.ReleaseData.FilterEntities();
            OnPropertyChanged(nameof(SearchText));
        }
    }
    
    private void TextBox_KeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Enter) {
            _query.ProcessSqlQuery();
            OnPropertyChanged(nameof(FilteredBooks));
            OnPropertyChanged(nameof(FilteredAuthors));
            OnPropertyChanged(nameof(FilteredReaders));
            OnPropertyChanged(nameof(FilteredReleases));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}