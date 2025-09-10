using System.Windows;
using System.Windows.Threading;

namespace ZipArchiver.ViewModels;

public partial class ProgressBarView : Window {
    public ProgressBarViewModel _viewModel;
    
    public ProgressBarView() {
        InitializeComponent();
        _viewModel = new ProgressBarViewModel();
        DataContext = _viewModel;
    }
    
    public void UpdateProgress(double progress) {
        _viewModel.Progress = progress;
        if (progress >= 100) Close();
    }
}