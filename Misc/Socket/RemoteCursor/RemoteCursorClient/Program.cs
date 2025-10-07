using RemoteCursor.Client;

namespace RemoteCursor;

internal static class Program {
    static void Main() {
        RemoteCursorClient client = new("127.x.x.x:6969");
        client.Run();
    }
}