using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ZipArchiver.ViewModels;

public class ZipViewModel : XViewModelBase {
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private string _sourcePath;
    private string _destinationPath;

    public string ZipSourcePath {
        get => _sourcePath;
        set {
            _sourcePath = $"{value}";
            OnPropertyChanged(nameof(ZipSourcePath));
        }
    }

    public string ZipDestinationPath {
        get => _destinationPath;
        set {
            _destinationPath =  $"{value}.zip";
            OnPropertyChanged(nameof(ZipDestinationPath));
        }
    }
}