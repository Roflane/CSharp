using System.Windows.Input;
using ZipArchiver.Commands;

namespace ZipArchiver.ViewModels;

public class MainViewModel : XViewModelBase {
    private XViewModelBase _selectedViewModel;

    public bool IsZipView => SelectedViewModel is ZipViewModel;
    public bool IsUnzipView => SelectedViewModel is UnzipViewModel;
    public XViewModelBase SelectedViewModel {
        get => _selectedViewModel;
        set {
            _selectedViewModel = value;
            OnPropertyChanged(nameof(SelectedViewModel));
            OnPropertyChanged(nameof(IsZipView));
            OnPropertyChanged(nameof(IsUnzipView));
        }
    }

    public ICommand UpdateViewCommand { get; set; } 

    public MainViewModel() {
        UpdateViewCommand = new UpdateViewCommand(this);
    }
}