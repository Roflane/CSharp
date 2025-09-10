using System.ComponentModel;

namespace ZipArchiver.ViewModels;

public class UnzipViewModel : XViewModelBase {
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private string _sourcePath;
    private string _destinationPath;

    public string UnzipSourcePath {
        get => _sourcePath;
        set {
            _sourcePath = $"{value}.zip";
            OnPropertyChanged(nameof(UnzipSourcePath));
        }
    }

    public string UnzipDestinationPath {
        get => _destinationPath;
        set {
            _destinationPath = value;
            OnPropertyChanged(nameof(UnzipDestinationPath));
        }
    }
}