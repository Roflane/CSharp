namespace RemoteCursorServer;

static class Program {
    static void Main() {
        var server = new RemoteCursor.Server.RemoteCursorServer(6969);
        server.Run();
    }
}