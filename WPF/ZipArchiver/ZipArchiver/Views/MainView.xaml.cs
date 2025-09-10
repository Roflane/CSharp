using System.Windows;
using ZipArchiver.ViewModels;

namespace ZipArchiver;

public partial class MainView : Window {
    private readonly string _title = "Zip Archiver";

    public MainView() {
        InitializeComponent();
        Title = _title;
        DataContext = new MainViewModel();
    }
}