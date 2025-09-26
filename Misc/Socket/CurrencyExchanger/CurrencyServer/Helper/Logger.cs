namespace CurrencyServer.Helper;

public static class Logger {
    private static Lock _lock = new Lock();
    
    public static void Write(string msg) {
        lock (_lock) {
            File.WriteAllText("log.txt", $"{msg}\n");
        }
    }
}