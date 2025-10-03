using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using XCloudRepo.Configs;
using XCloudRepo.Core;
using XCloudRepo.Enums;
using XCloudRepo.Internals;
using XCloudRepo.ResponseHandler;

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
        XResponseHandler rh = new();
        XCloudFunc func = new();
        XBuffer xb = new();
        try {
            while (client.Connected) {
                if (!accountManager.IsAuthorized) {
                    string enterRequest = Encoding.UTF8.GetString(xb.EnterBuffer, 0, client.Receive(xb.EnterBuffer));
                    string userDataFormat = Encoding.UTF8.GetString(xb.UserDataBuffer, 0, client.Receive(xb.UserDataBuffer));

                    switch (enterRequest) {
                        case XCloudServerConfig.Register:
                            bool registerStatus = accountManager.RegisterUser(userDataFormat).Result;
                            PLog.Register(registerStatus, client.RemoteEndPoint!.ToString(), accountManager.UserLogin);
                            break;
                        case XCloudServerConfig.Auth:
                            bool authStatus = accountManager.AuthUser(userDataFormat).Result;
                            PLog.Auth(authStatus, client.RemoteEndPoint!.ToString(), accountManager.UserLogin);
                            break;
                    }
                    client.Send(Encoding.UTF8.GetBytes(accountManager.IsAuthorized.ToString()));
                }
                else {
                    XCloudCore core = new(accountManager.UserLogin);
                    
                    string request = Encoding.UTF8.GetString(xb.RequestBuffer, 0, client.Receive(xb.RequestBuffer));
                    switch (request) {
                        case XCloudServerConfig.DirectoryViewRoot:
                            PLog.DirectoryViewRoot(func.SerializeRootDir(client, core) > 0, 
                                client.RemoteEndPoint!.ToString());
                            break;
                        case XCloudServerConfig.DirectoryCreate:
                            string dirToCreate = func.ReceiveString(client, xb.DirToCreateBuffer);
                            PLog.DirectoryCreate(core.DirectoryCreate(dirToCreate), 
                                client.RemoteEndPoint!.ToString(), dirToCreate);
                            break;
                        case XCloudServerConfig.DirectoryDelete:
                            string dirDelete = func.ReceiveString(client, xb.DirToDeleteBuffer);
                            PLog.DirectoryDelete(core.DirectoryDelete(dirDelete), 
                                client.RemoteEndPoint!.ToString(), dirDelete);
                            break;
                        case XCloudServerConfig.DirectoryRename:
                            string oldDir = func.ReceiveString(client, xb.OldDirBuffer);
                            string newDir = func.ReceiveString(client, xb.NewDirBuffer);
                            PLog.DirectoryRename(core.DirectoryRename(oldDir, newDir), client.RemoteEndPoint!.ToString(), oldDir, newDir);
                            break;
                        case XCloudServerConfig.FileUpload:
                            string dirToUpload = func.ReceiveString(client, xb.DirToUploadBuffer);
                            if (!rh.DirectoryExistance(core, dirToUpload, client,
                                    () => { Log.Red("Directory doesn't exist."); })) {
                                continue;
                            }
                            
                            string fileName = func.ReceiveString(client, xb.FileNameBuffer);
                            if (fileName == XReservedData.InvalidName) continue;

                            client.Receive(xb.FileSizeBuffer);
                            long fileSize = BitConverter.ToInt64(xb.FileSizeBuffer, 0);
                            if (!rh.FileSize(fileSize, client,
                                    () => { Log.Red("File size overflow."); })) {
                                continue;
                            }

                            xb.ExpandFileToUploadBuffer(fileSize);
                            client.Receive(xb.FileToUploadBuffer);
                            PLog.FileUpload(core.FileUpload(dirToUpload, fileName, xb.FileToUploadBuffer).Result, 
                                client.RemoteEndPoint!.ToString(), fileName);
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