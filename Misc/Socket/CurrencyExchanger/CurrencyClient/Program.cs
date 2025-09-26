class Program {
    static void Main(string[] args) {
        Client client = new("127.0.0.1:4773");
        client.Run();
        
        AutoResetEvent ev = new(false);
        ev.WaitOne();
    }
}