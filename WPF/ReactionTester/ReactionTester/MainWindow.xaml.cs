using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ReactionTester;

public partial class MainWindow : Window {
    private Stopwatch _stopwatch = new();
    private DispatcherTimer _timer = new();
    private DispatcherTimer _colorChangeTimer;
    private Button _mainButton;
    
    public MainWindow() {
        InitializeComponent();
        Title = "Reaction Tester";
        
        _timer.Interval = TimeSpan.FromMilliseconds(50);
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }
    
    private void Timer_Tick(object sender, EventArgs e) {
        if (_mainButton != null) {
            UpdateButtonText(ReactionTesterCore.Instance.StatusMessage);
        }
    }
    
    private void Button_OnClick(object sender, RoutedEventArgs e) {
        if (sender is Button button) {
            _mainButton = button;
            var core = ReactionTesterCore.Instance;
            
            switch (core.State) {
                case EState.None:
                    core.StartWaiting();
                    StartColorChangeTimer();
                    break;
                    
                case EState.Waiting:
                    if (button.Background != Brushes.Green) {
                        _colorChangeTimer?.Stop();
                        core.SetTooSoon();
                        button.Background = Brushes.Red;
                    }
                    else {
                        _stopwatch.Stop();
                        core.SetResult(_stopwatch.ElapsedMilliseconds);
                        button.Background = Brushes.LightGray;
                    }
                    break;
                    
                case EState.TooSoon:
                case EState.Result:
                    core.ResetTest();
                    button.Background = Brushes.LightGray;
                    break;
            }
            
            UpdateButtonText(core.StatusMessage);
        }
    }
    
    private void StartColorChangeTimer() {
        _colorChangeTimer?.Stop();
        _colorChangeTimer = new DispatcherTimer();
        _colorChangeTimer.Interval = TimeSpan.FromMilliseconds(new Random().Next(1000, 4000));
        _colorChangeTimer.Tick += (s, e) => {
            _colorChangeTimer.Stop();
            if (ReactionTesterCore.Instance.State == EState.Waiting && _mainButton != null) {
                _mainButton.Background = Brushes.Green;
                _stopwatch.Restart();
            }
        };
        _colorChangeTimer.Start();
    }
    
    private void UpdateButtonText(string text) {
        if (_mainButton != null) {
            var textBlock = FindVisualChild<TextBlock>(_mainButton);
            if (textBlock != null) {
                textBlock.Text = text;
            }
        }
    }
    
    private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++) {
            DependencyObject child = VisualTreeHelper.GetChild(obj, i);
            if (child is T result)
                return result;
            else {
                T childOfChild = FindVisualChild<T>(child);
            }
        }
        return null;
    }
}