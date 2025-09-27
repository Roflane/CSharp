namespace CurrencyServer.Helper;

public static class Logger {
    private static readonly Lock Lock = new();
    
    public static void Write(string msg) {
        lock (Lock) {
            File.AppendAllText("log.txt", $"{msg}\n");
        }
    }
}