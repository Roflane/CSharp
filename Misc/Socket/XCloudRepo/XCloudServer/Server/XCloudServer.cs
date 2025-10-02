using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using XCloudRepo.Configs;
using XCloudRepo.Core;
using XCloudRepo.Enums;

namespace XCloudRepo.Server;

public class XCloudServer : IDisposable {
    private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private readonly IPEndPoint _ep;
    
    public XCloudServer(string ipPort) {
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
                    
                    string clientLog = $"[{client.RemoteEndPoint}] Connected at {DateTime.Now}";
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
                                Log.Green($"[{client.RemoteEndPoint}] User {accountManager.UserLogin} registered successfully");
                            }
                            else {
                                Log.Red($"[{client.RemoteEndPoint}] User {accountManager.UserLogin} registered unsuccessfully");
                            }

                            break;
                        case XCloudServerConfig.Auth:
                            if (accountManager.AuthUser(userDataFormat).Result) {
                                Log.Green($"[{client.RemoteEndPoint}] User {accountManager.UserLogin} authorized successfully");
                            }
                            else {
                                Log.Red($"[{client.RemoteEndPoint}] User authorized unsuccessfully");
                            }
                            break;
                    }

                    client.Send(Encoding.UTF8.GetBytes(accountManager.IsAuthorized.ToString()));
                }
                else {
                    XCloudCore core = new(accountManager.UserLogin);

                    byte[] requestBuffer = new byte[1024];
                    int length = client.Receive(requestBuffer);
                    string request = Encoding.UTF8.GetString(requestBuffer, 0, length);
                    
                    switch (request) {
                        case XCloudServerConfig.DirectoryViewRoot:
                            string[] dirs = core.DirectoryViewRoot();
                            string json = JsonConvert.SerializeObject(dirs);
                            await client.SendAsync(Encoding.UTF8.GetBytes(json));
                            Log.Green("Request 'DirectoryViewRoot' accomplished.");
                            break;
                        case XCloudServerConfig.DirectoryCreate:
                            byte[] dirCreateBuffer = new byte[1024];
                            int incomingBufferLength = await client.ReceiveAsync(dirCreateBuffer);
                            string incomingDirCreate = Encoding.UTF8.GetString(dirCreateBuffer, 0, incomingBufferLength);
                            
                            if (core.DirectoryCreate(incomingDirCreate)) {
                                Log.Green("Request 'DirectoryCreate' succeeded.");
                            }
                            else Log.Red("Request 'DirectoryCreate' unsucceeded.");
                            break;
                        case XCloudServerConfig.DirectoryDelete:
                            byte[] dirDeleteBuffer = new byte[1024];
                            int dirDeleteBufferLength = await client.ReceiveAsync(dirDeleteBuffer);
                            string dirDelete = Encoding.UTF8.GetString(dirDeleteBuffer, 0, dirDeleteBufferLength);
                            
                            if (core.DirectoryDelete(dirDelete)) {
                                Log.Green($"Request 'DirectoryDelete' succeeded: {dirDelete}.");
                            }
                            else Log.Red($"Request 'DirectoryDelete' unsucceeded: {dirDelete}.");
                            break;
                        case XCloudServerConfig.DirectoryRename:
                            byte[] oldDirBuffer = new byte[260];
                            byte[] newDirBuffer = new byte[260];
                            
                            string oldDir = Encoding.UTF8.GetString(oldDirBuffer, 0, client.Receive(oldDirBuffer));
                            string newDir = Encoding.UTF8.GetString(newDirBuffer, 0, client.Receive(newDirBuffer));
                            if (oldDir.Length == 0 || newDir.Length == 0) Log.Green("Request 'DirectoryRename' unsucceeded: invalid length.");
                            
                            
                            
                            if (core.DirectoryRename(oldDir, newDir)) {
                                Log.Green("Request 'DirectoryRename' succeeded.");
                            }
                            else Log.Red("Request 'DirectoryRename' unsucceeded.");
                            break;
                        case XCloudServerConfig.FileUpload:
                            Log.Green("'FileUpload'.\n");
                            
                            byte[] dirToUploadBuffer = new byte[260];
                            byte[] fileNameBuffer = new byte[100];
                            byte[] fileSizeBuffer = new byte[100];
                            
                            string dirToUpload = Encoding.UTF8.GetString(dirToUploadBuffer, 0, client.Receive(dirToUploadBuffer));
                            string fileName = Encoding.UTF8.GetString(fileNameBuffer, 0, client.Receive(fileNameBuffer));
                            int fileSize = int.Parse(Encoding.UTF8.GetString(fileSizeBuffer, 0, client.Receive(fileSizeBuffer)));
                            if (fileSize > XCloudServerConfig.MaxFileBufferSize) {
                                client.Send(Encoding.UTF8.GetBytes($"{EResponseCode.FileSizeOverflow}"));
                                continue;
                            } 
                            client.Send(Encoding.UTF8.GetBytes($"{EResponseCode.FileSizeOk}"));
                            
                            byte[] fileToUploadBuffer = new byte[fileSize];
                            client.Receive(fileToUploadBuffer);
    
                            if (core.FileUpload(dirToUpload, fileName, fileToUploadBuffer).Result) {
                                Log.Green("Request 'FileUpload' succeeded.");
                            }
                            else Log.Red("Request 'FileUpload' unsucceeded.");
                            break;
                        case XCloudServerConfig.FileDelete:
                            // if (core.FileDelete(incomingData)) {
                            //     Log.Green("Request 'FileDelete' succeeded.");
                            // }
                            // else Log.Red("Request 'FileDelete' unsucceeded.");

                            break;
                        case XCloudServerConfig.FileRename:
                            // if (core.FileRename(dirParts[0], dirParts[1])) {
                            //     Log.Green("Request 'FileRename' succeeded.");
                            // }
                            // else Log.Red("Request 'FileRename' unsucceeded.");

                            break;
                    }
                }
            }
        }
        catch {}
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