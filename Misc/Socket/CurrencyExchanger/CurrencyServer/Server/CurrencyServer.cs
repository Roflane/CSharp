using System.Net;
using System.Text;
using System.Net.Sockets;
using CurrencyServer.Helper;
using System.Diagnostics.CodeAnalysis;
using CurrencyServer.Parser;
using HtmlAgilityPack;

public class Server : IDisposable {
    //private Semaphore _semaphore;
    private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private IPEndPoint _ep;
    
    private HtmlWeb _web = new();
    
    public Server(string ipPort) {
      //  _semaphore = new Semaphore(3, 3, "CurrencyServer");
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
                    
                    // if (_semaphore.WaitOne(0)) {
                    //     client.Close();
                    //     break;
                    // }
                    
                    byte[] statusBuffer = new byte[1024];
                    int statusData = client.Receive(statusBuffer);
                    Log.Blue($"{statusData}\n");                    
        
                    string clientLog = $"Client {client.RemoteEndPoint} connected at {DateTime.Now}";
                    Log.Green($"\n{clientLog}");
                    Logger.Write(clientLog);
                    
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
        
            int requestData = client.Receive(buffer);
            while (client.Connected) {
                string requestText = Encoding.UTF8.GetString(buffer, 0, requestData);
                string[] tokens = requestText.Trim().Split(' ');
                
                if (tokens.Length != 5) {
                    Log.Red($"Invalid token count: {tokens.Length} for request: '{requestText}'\n");
                    continue;
                }
            
                string from = tokens[0];
                string amount = tokens[2];
                string to = tokens[4];
                    
                try {
                    string res = XeParser.GetResult(_web, amount, from, to);
                    client.Send(Encoding.UTF8.GetBytes(res));
                }
                catch (Exception ex) {
                    Log.Red($"Send error: {ex.Message}\n");
                    Log.Red($"Stack trace: {ex.StackTrace}\n");
                }
            }
        }
        catch (Exception ex) {
            Log.Red($"Client error: {ex.Message}");
        }
        finally {
            client.Close();
            //_semaphore.Release();
        }
    }

    public void Dispose() {
        _socket.Close();
        _socket.Dispose();
        //_semaphore.Dispose();
    }
}