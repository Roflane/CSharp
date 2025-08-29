using System.Windows;

namespace AppLauncher;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        Name = "AppLauncher";
        
        if (Checker.IsAppRunning(Name, 1)) this.Close();
        LauncherCore.Instance.PushArgs();
    }
    
    private void ButtonClose_OnClick(object sender, RoutedEventArgs e) {
        Application.Current.Shutdown();
    }

    private void ButtonStart_OnClick(object sender, RoutedEventArgs e) {
        LauncherCore.Instance.LaunchApp();
    }
}