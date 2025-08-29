using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using TaskManager.Core;
namespace TaskManager;

public static class WrapperEvent {
    public static void Priority(object sender, ProcessPriorityClass priority) {
        try {
            if (sender is MenuItem menuItem) {
                if (menuItem.CommandParameter is ProcessInfo processInfo) {
                    TaskManagerCore.Instance.ChangePriority(processInfo.ProcessId, priority);
                }
            }
        }
        catch (Exception ex) {
            MessageBox.Show($"{ex.Message}",  "Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
public partial class MainWindow : Window {
    private DispatcherTimer _timer = new DispatcherTimer();
    public MainWindow() {
        InitializeComponent();
         
        DataContext = TaskManagerCore.Instance;
        ProcessListBox.ItemsSource = TaskManagerCore.Instance.Processes;
        
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        timer.Tick += (s, e) => TaskManagerCore.Instance.GetProcesses();
        timer.Start();
    }
    
    private void ButtonClose_OnClick(object sender, RoutedEventArgs e) {
        Application.Current.Shutdown();
    }

    private void Kill_OnClick(object sender, RoutedEventArgs e) {
        try {
            if (sender is MenuItem menuItem) {
                if (menuItem.CommandParameter is ProcessInfo processInfo) {
                    TaskManagerCore.Instance.KillProcess(processInfo.ProcessId);
                }
            }
        }
        catch (Exception ex) {
            MessageBox.Show($"{ex.Message}",  "Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void PriorityRealtime_OnClick(object sender, RoutedEventArgs e) {
        WrapperEvent.Priority(sender, ProcessPriorityClass.RealTime);
    }
    
    private void PriorityHigh_OnClick(object sender, RoutedEventArgs e) {
        WrapperEvent.Priority(sender, ProcessPriorityClass.High);
    }

    private void PriorityAboveNormal_OnClick(object sender, RoutedEventArgs e) {
        WrapperEvent.Priority(sender, ProcessPriorityClass.AboveNormal);
    }
    
    private void PriorityNormal_OnClick(object sender, RoutedEventArgs e) {
        WrapperEvent.Priority(sender, ProcessPriorityClass.Normal);
    }
    
    private void PriorityBelowNormal_OnClick(object sender, RoutedEventArgs e) {
        WrapperEvent.Priority(sender, ProcessPriorityClass.BelowNormal);
    }
    
    private void PriorityIdle_OnClick(object sender, RoutedEventArgs e) {
        WrapperEvent.Priority(sender, ProcessPriorityClass.Idle);
    }

    private void AffinityOne_OnClick(object sender, RoutedEventArgs e) {
        try {
            if (sender is MenuItem menuItem) {
                if (menuItem.CommandParameter is ProcessInfo processInfo) {
                    TaskManagerCore.Instance.SetAffinityOne(processInfo.ProcessId);
                }
            }
        }
        catch (Exception ex) {
            MessageBox.Show($"{ex.Message}",  "Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void AffinityFull_OnClick(object sender, RoutedEventArgs e) {
        try {
            if (sender is MenuItem menuItem) {
                if (menuItem.CommandParameter is ProcessInfo processInfo) {
                    TaskManagerCore.Instance.SetAffinityFull(processInfo.ProcessId);
                }
            }
        }
        catch (Exception ex) {
            MessageBox.Show($"{ex.Message}",  "Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}