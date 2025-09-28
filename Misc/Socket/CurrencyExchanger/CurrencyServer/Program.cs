namespace CurrencyServer;

class Program {
    static void Main() {
        Server server = new("127.0.0.1:4773");
        server.BeginListening();
        
        AutoResetEvent ev = new(false);
        ev.WaitOne();        
    }
}