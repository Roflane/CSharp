using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

class Program {
    static void Main(string[] args) {
        ServerSocket ss = new("127.0.0.1:4773");
        ss.BeginListening();
        
        AutoResetEvent autoEvent = new AutoResetEvent(false);
        autoEvent.WaitOne();
    }
}

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
 


class ServerSocket : IDisposable {
    private int _counter;
    
    private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private IPEndPoint _ep;
    
    // private ConcurrentBag<Socket> _clients = [];
    private Dictionary<int, string> _clientsData = [];
    
    public ServerSocket(string ipPort) {
        _ep = IPEndPoint.Parse(ipPort);
        _socket.Bind(_ep);
        _socket.Listen();
        PrintInfo();
    }

    private void PrintInfo() {
        Log.Red($"""
                _____________________________________
                Address: {_ep.Address}
                Port: {_ep.Port}
                Address Family: {_ep.AddressFamily}
                _____________________________________
                """);
    } 
    
    
    [DoesNotReturn]
    public void BeginListening() {
        _ = Task.Run(() => {
            Log.Blue("Waiting for client...");
        
            while (true) {
                try {
                    Socket client = _socket.Accept();
                    Log.Green("\nClient connected");
                
                    _ = Task.Run(() => HandleClient(client));
                }
                catch (Exception ex) {
                    Log.Red($"Accept error: {ex.Message}");
                    break;
                }
            }
        });
    }

    private void HandleClient(Socket client) {
        try {
            byte[] buffer = new byte[1024];
            
            int nicknameData = client.Receive(buffer);
            if (nicknameData > 0) {
                string nickname = Encoding.UTF8.GetString(buffer, 0, nicknameData);
                _clientsData.Add(++_counter, nickname);
                Log.Green($"Established connection with {nickname}.");
            
                while (client.Connected) {
                    int messageData = client.Receive(buffer);
                    if (messageData == 0) break;
                
                    string message = Encoding.UTF8.GetString(buffer, 0, messageData);
                    Log.Green($"{nickname}: {message}");
                }
            }
        }
        catch (Exception ex) {
            Log.Red($"Client error: {ex.Message}");
        }
        finally {
            client.Close();
        }
    }

    public void Dispose() {
        _socket.Close();
        _socket.Dispose();
    }
}