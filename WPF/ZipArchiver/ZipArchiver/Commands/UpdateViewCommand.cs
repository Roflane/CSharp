using System.Windows.Input;
using ZipArchiver.ViewModels;

namespace ZipArchiver.Commands;

public class UpdateViewCommand : ICommand {
    public event EventHandler? CanExecuteChanged;
    
    private MainViewModel _viewModel;
    
    public UpdateViewCommand(MainViewModel viewModel) {
        _viewModel = viewModel;
    }
    
    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) {
        switch (parameter?.ToString()) {
            case "Zip":
                _viewModel.SelectedViewModel = new ZipViewModel();
                break;
            case "Unzip":
                _viewModel.SelectedViewModel = new UnzipViewModel();
                break;
        }
    }
    
}