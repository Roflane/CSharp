namespace CurrencyServer.Helper;

static class Log {
    public static void Red(string msg) {
        Console.WriteLine($"\u001b[31m{msg}\u001b[0m");
    }
    
    public static void Green(string msg) {
        Console.WriteLine($"\u001b[32m{msg}\u001b[0m");
    }
    
    public static void Blue(string msg) {
        Console.WriteLine($"\u001b[34m{msg}\u001b[0m");
    }
}