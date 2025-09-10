using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ZipArchiver.ViewModels;

public class XViewModelBase : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}