using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZipArchiver.ViewModels;

public class ProgressBarViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private int _count;
    private double _progress;
    public double Progress {
        get => _progress;
        set {
            _progress = value;
            OnPropertyChanged(nameof(Progress));
        }
    }

    public int Count {
        get => _count;
        set {
            _count = value;
            OnPropertyChanged(nameof(Count));
        }
    }
}