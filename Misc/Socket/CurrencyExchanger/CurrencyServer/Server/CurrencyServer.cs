using System.Net;
using System.Text;
using System.Net.Sockets;
using CurrencyServer.Helper;
using System.Diagnostics.CodeAnalysis;
using CurrencyServer.Parser;
using HtmlAgilityPack;

public class Server : IDisposable {
    private Semaphore _semaphore;
    private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private IPEndPoint _ep;
    
    private HtmlWeb _web = new();
    
    public Server(string ipPort) {
        _semaphore = new Semaphore(10, 10, "CurrencyServer");
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

                    if (!CurrencyServerChecker.InitCheckAndProcess(_semaphore, client)) break;
                    
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
            while (client.Connected) {
                byte[] buffer = new byte[1024];
                int requestData = client.Receive(buffer);
                
                string requestText = Encoding.UTF8.GetString(buffer, 0, requestData);
                string[] tokens = requestText.Trim().Split(' ');
                
                if (tokens.Length != 5) {
                    Log.Red($"Invalid token count: {tokens.Length} for request: '{requestText}'\n");
                    continue;
                }
            
                string from = tokens[1];
                string amount = tokens[2];
                string to = tokens[4];
                    
                try {
                    string? res = XeParser.GetResult(amount, from, to);
                    if (res != null)  {
                        client.Send(Encoding.UTF8.GetBytes(res));
                        string resMsg = $"[{DateTime.Now}] Sent: {res} to {client.RemoteEndPoint}";
                        Log.Blue(resMsg);
                        Logger.Write(resMsg);
                    }
                    else {
                        client.Send(Encoding.UTF8.GetBytes("Invalid token found, please try again."));
                    }
                }
                catch (Exception ex) {
                    Log.Red($"Send error: {ex.Message}\n");
                    Log.Red($"Stack trace: {ex.StackTrace}\n");
                }
            }
        }
        finally {
            string clientLog = $"Client {client.RemoteEndPoint} disconnected at {DateTime.Now}";
            Log.Red($"[{DateTime.Now}] {clientLog}\n");
            Logger.Write(clientLog);
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