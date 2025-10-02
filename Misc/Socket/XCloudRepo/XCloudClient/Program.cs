namespace XCloudClient;

class Program {
    static void Main() {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        Client.XCloudClient client = new("192.168.31.121:4773");
        client.Run();
    }
}