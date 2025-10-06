using XCloudRepo.Server;

class Program {
    static async Task Main() {
        XCloudServer server = new("192.168.31.121:4773"); 
        Console.Title = $"[{server.Ip}:{server.Port}] XCloud server";
        await server.BeginListening();
    }
}