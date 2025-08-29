using System.Diagnostics;

public static class Checker {
    public static bool IsAppRunning(string app, int count = 0) {
        var res = Process.GetProcessesByName(app);
        return res.Length > count;
    }
}