using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using XCloudRepo.Configs;
using XCloudRepo.Core;

namespace XCloudRepo.Server;

public class XCloudServer : IDisposable {
    private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private IPEndPoint _ep;
    
    public XCloudServer(string ipPort) {
        _ep = IPEndPoint.Parse(ipPort);
        _socket.Bind(_ep);
        _socket.Listen();
    }
    
    [DoesNotReturn]
    public void BeginListening() {
        _ = Task.Run(() => {
            Log.Blue("Waiting for client...");
        
            while (true) {
                try {
                    Socket client = _socket.Accept();
                    
                    string clientLog = $"Client {client.RemoteEndPoint} connected at {DateTime.Now}";
                    Log.Green($"\n{clientLog}");

                    _ = Task.Run(() => HandleClient(client));
                }
                catch (Exception ex) {
                    Log.Red($"Accept error: {ex.Message}");
                    break;
                }
            }
        });
    }
    
    private async void HandleClient(Socket client) {
        XCloudAccountCore accountManager = new();
        try {            
            while (client.Connected) {
                if (!accountManager.IsAuthorized) {
                    byte[] enterBuffer = new byte[100];
                    byte[] userDataBuffer = new byte[XRegistrationConfig.MaxLoginLength + XRegistrationConfig.MaxLoginLength];
                
                    string enterRequest = Encoding.UTF8.GetString(enterBuffer, 0, client.Receive(enterBuffer));
                    string userDataFormat = Encoding.UTF8.GetString(userDataBuffer, 0, client.Receive(userDataBuffer));
            
                    switch (enterRequest) {
                        case XCloudServerConfig.Register:
                            if (accountManager.RegisterUser(userDataFormat).Result) {
                                Log.Green($"User {accountManager.UserLogin} registered successfully");
                            }
                            else Log.Red($"User {accountManager.UserLogin} registered unsuccessfully");
                            break;
                        case XCloudServerConfig.Auth:
                            if (accountManager.RegisterUser(userDataFormat).Result) {
                                Log.Green($"User {accountManager.UserLogin} authorized successfully");
                            }
                            else Log.Red($"User {accountManager.UserLogin} authorized unsuccessfully");
                            break;
                    }
                }
                else {
                    XCloudCore core = new(accountManager.UserLogin);
                    
                    byte[] requestBuffer = new byte[1024];
                    int length = await client.ReceiveAsync(requestBuffer);
                    string request = Encoding.UTF8.GetString(requestBuffer,0, length);
                    
                    byte[] mainBuffer = new byte[260 * 2 + 1 + XCloudServerConfig.Reserved.Length];
                    int mainBufferLength = await client.ReceiveAsync(requestBuffer);
                    string mainData = Encoding.UTF8.GetString(mainBuffer,0, mainBufferLength);
                    string[] dirParts = mainData.Split(XCloudServerConfig.Reserved);
                    
                    byte[] incomingBuffer = new byte[1024 * 100];
                    int incomingBufferLength = await client.ReceiveAsync(incomingBuffer);
                    string incomingData = Encoding.UTF8.GetString(incomingBuffer, 0, incomingBufferLength);
                    
                    switch (request) {
                        case XCloudServerConfig.DirectoryViewRoot:
                            string[] dirs = core.DirectoryViewRoot();
                            string json = JsonSerializer.Serialize(dirs);
                            client.Send(Encoding.UTF8.GetBytes(json));
                            Log.Green("Request 'DirectoryViewRoot' accomplished.");
                            break;
                        case XCloudServerConfig.DirectoryCreate:
                            if (core.DirectoryCreate(mainData)) {
                                Log.Green("Request 'DirectoryCreate' succeeded.");
                            } else Log.Red("Request 'DirectoryCreate' unsucceeded.");
                            break;
                        case XCloudServerConfig.DirectoryDelete:
                            if (core.DirectoryDelete(mainData)) {
                                Log.Green("Request 'DirectoryDelete' succeeded.");
                            } else Log.Red("Request 'DirectoryDelete' unsucceeded.");
                            break;
                        case XCloudServerConfig.DirectoryRename:
                            if (dirParts.Length != 2) Log.Red("Request 'DirectoryRename' failed due to invalid dir data.");

                            if (core.DirectoryRename(dirParts[0], dirParts[1])) {
                                Log.Green("Request 'DirectoryRename' succeeded.");
                            } else Log.Red("Request 'DirectoryRename' unsucceeded.");
                            break;
                        case XCloudServerConfig.FileUpload:
                            if (core.FileUpload(mainData, incomingBuffer).Result) {
                                Log.Green("Request 'FileUpload' succeeded.");
                            } else  Log.Red("Request 'FileUpload' unsucceeded.");
                            break;
                        case XCloudServerConfig.FileDelete:
                            if (core.FileDelete(incomingData)) {
                                Log.Green("Request 'FileDelete' succeeded.");
                            } else  Log.Red("Request 'FileDelete' unsucceeded.");
                            break;
                        case XCloudServerConfig.FileRename:
                            if (core.FileRename(dirParts[0], dirParts[1])) {
                                Log.Green("Request 'FileRename' succeeded.");
                            } else Log.Red("Request 'FileRename' unsucceeded.");
                            break;
                    }
                }
            }
        }
        finally {
            string clientLog = $"Client {client.RemoteEndPoint} disconnected at {DateTime.Now}";
            Log.Red($"[{DateTime.Now}] {clientLog}\n");
            client.Close();
        }
    }

    public void Dispose() {
        _socket.Close();
        _socket.Dispose();
    }
}