static class Log {
    public static void Red(string msg) {
        Console.Write($"\u001b[31m{msg}\u001b[0m");
    }
    
    public static void Green(string msg) {
        Console.Write($"\u001b[32m{msg}\u001b[0m");
    }
    
    public static void Blue(string msg) {
        Console.Write($"\u001b[34m{msg}\u001b[0m");
    }
    
    public static void Magenta(string msg) {
        Console.Write($"\u001b[35m{msg}\u001b[0m");
    }
}