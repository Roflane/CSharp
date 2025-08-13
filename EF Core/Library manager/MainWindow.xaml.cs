using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using LibraryManager.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LibraryManager;

public partial class MainWindow : Window {
    private readonly string _wndName = "LibraryManager";
    
    private readonly LibraryContext _dbContext;
    private string _searchText;
    private List<Book> _books;
    private List<Book> _filteredBooks;
    
    public MainWindow() {
        InitializeComponent();
        this.Title = _wndName;
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("sqlsettings.json")
            .Build();

        var options = new DbContextOptionsBuilder<LibraryContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"))
            .Options;

        _dbContext = new LibraryContext(options);
        DataContext = this;
        
        LoadBooks();
    }
    
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged(nameof(SearchText));
            FilterBooks();
        }
    }
    
    public List<Book> FilteredBooks
    {
        get => _filteredBooks;
        set {
            _filteredBooks = value;
            OnPropertyChanged(nameof(FilteredBooks));
        }
    }
    
    private void LoadBooks()
    {
        _books = _dbContext.Books.ToList();
        FilteredBooks = _books;
    }
    
    private void FilterBooks()
    {
        if (string.IsNullOrWhiteSpace(SearchText)) {
            FilteredBooks = new List<Book>();
        }
        else {
            FilteredBooks = _books
                .Where(b => b.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
    
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
        FilterBooks();
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}