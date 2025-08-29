using System.Diagnostics;
using System.Windows;

public class LauncherCore {
    private static LauncherCore? _instance = null;
    
    private bool _isDebug = false;
    private readonly string _app = "python";
    private string[] _args;
    
    public static LauncherCore Instance {
        get => _instance ??= new LauncherCore();
    }
    
    public void PushArgs() {
        _args = Environment.GetCommandLineArgs();
    }

    private string? ExtractMathOp() {
        string? innerExpression = _args.LastOrDefault();
        return innerExpression;
    }
    
    public void LaunchApp() {
        if (!Checker.IsAppRunning(_app)) {
            string targetArg;
            char[] allowed = "0123456789+-*/.() ".ToCharArray();
            bool valid = ExtractMathOp()!.All(c => allowed.Contains(c));
            
            if (!valid) {
                targetArg = $"-c \"with open('log.txt', 'a') as f: f.write('{Response.ErrorParse}')\"";
                MessageBox.Show("Failed to parse arg.");
            }
            else targetArg = $"-i -c \"print({ExtractMathOp()})\"";
              
            
            Process? proc = Process.Start(new ProcessStartInfo {
                FileName = _app, 
                Arguments = targetArg,
                UseShellExecute = true
            });
            
            if (proc == null) {
                MessageBox.Show($"{Response.ErrorParse}", "AppLauncher", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else MessageBox.Show($"{Response.ErrorAppIsRunning}", "AppLauncher", 
            MessageBoxButton.OK, MessageBoxImage.Information);
    }
}