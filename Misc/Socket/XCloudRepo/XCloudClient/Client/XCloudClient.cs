using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text;
using XCloudClient.Configs;
using XCloudClient.Core;
using XCloudClient.Data;
using XCloudClient.Enums;
using XCloudClient.Internals;
using XCloudClient.Menu;
using XCloudClient.ResponseHandler;
using XCloudClient.User;

namespace XCloudClient.Client;

public class XCloudClient {
    private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private readonly UserData _userData = new();
    private readonly XCloudData _cloudData = new();
    
    public XCloudClient(string ipPort) {
        EstablishConnection(ipPort);
    }

    private void EstablishConnection(string ipPort) {
        var ep = IPEndPoint.Parse(ipPort);
        _socket.Connect(ep);
    }

    [DoesNotReturn]
    public async void Run() {
        XResponseHandler rh = new(_socket);
        XCloudFunc func = new(_socket);
        XBuffer xb = new();
        while (true) {
            if (!_userData.IsAuthorized) {
                XMenu.PrintEnterOptions();
                
                Log.Green("\nEnter option: ");
                string enter = Console.ReadLine()!;
                _socket.Send(Encoding.UTF8.GetBytes(enter));
                
                Log.Green("Enter login: ");
                string login = Console.ReadLine()!;
                if (login.Length > XRegistrationConfig.MaxDataLength) {
                    Log.Red($"Exceeded maximum length of {XRegistrationConfig.MaxDataLength} for login\n");
                    continue;
                }
                
                Log.Green("Enter password: ");
                string password = Console.ReadLine()!;
                if (password.Length > XRegistrationConfig.MaxDataLength + 1) {
                    Log.Red($"Exceeded maximum length of {XRegistrationConfig.MaxDataLength} for password\n");
                    continue;
                }

                string combinedData = $"{login}:{password}";
                _socket.Send(Encoding.UTF8.GetBytes(combinedData));
                
                int enterStatusData = _socket.Receive(xb.EnterStatusBuffer);
                string enterStatus = Encoding.UTF8.GetString(xb.EnterStatusBuffer, 0, enterStatusData);
                if (enterStatus == "True") {
                    Log.Green($"Welcome to XCloud, {_userData.IsAuthorized}!\n");
                    _userData.IsAuthorized = true;
                }
                else Console.Clear();
            }
            else {
                Console.Clear();
                XMenu.PrintCloudOptions();

                Log.Green("\nEnter option: ");
                string option = Console.ReadLine()!;
                _socket.Send(Encoding.UTF8.GetBytes(option));

                byte[] buffer = new byte[1024 * 10];
                
                switch (option) {
                    case XCloudClientConfig.DirectoryViewRoot: 
                        foreach (var dir in func.DeserializeRootDir(buffer)) {
                            Log.Blue($"{dir.Replace(@"\", "/")}\n");
                        }
                        Log.Blue("Enter any key to continue.", true);
                        break;
                    case XCloudClientConfig.DirectoryCreate:
                        Log.Green("Enter remote directory to create: ");
                        _socket.Send(Encoding.UTF8.GetBytes(Console.ReadLine()!));
                        break;
                    case XCloudClientConfig.DirectoryDelete:
                        Log.Green("Enter remote directory to delete: ");
                        _socket.Send(Encoding.UTF8.GetBytes(Console.ReadLine()!));
                        break;
                    case XCloudClientConfig.DirectoryRename:
                        Log.Green("Enter current directory name: ");
                        _socket.Send(Encoding.UTF8.GetBytes(Console.ReadLine()!));
                        
                        Log.Green("Enter new directory name: ");
                        _socket.Send(Encoding.UTF8.GetBytes(Console.ReadLine()!));
                        break;
                    case XCloudClientConfig.FileUpload:
                         Log.Green("Enter remote directory to upload a file: ");
                        string remoteDir = Console.ReadLine()!;
                        _socket.Send(Encoding.UTF8.GetBytes(remoteDir));

                        EResponseCode remoteDirStatus = func.ReceiveData(xb.StatusBuffer);
                        if (remoteDirStatus == EResponseCode.DirNotExists) continue;

                        Log.Green("Enter file path to upload: ");
                        string myDir = Console.ReadLine()!;

                        try {
                            FileInfo fi = new FileInfo(myDir);
                            if (!rh.LocalFileExistence(fi.Exists, fi.Name, () => {
                                    Log.Red("File doesn't exist\nPress any key to continue", true);
                                })) continue;
                            _socket.Send(BitConverter.GetBytes(fi.Length));
                            
                            EResponseCode status = func.ReceiveData(xb.StatusBuffer);
                            if (!rh.FileSize(status, myDir, () => PLog.FileSize(status))) continue;
                        }
                        catch (Exception ex) {
                            Log.Red($"Error: {ex.Message}");
                        }
                        break;
                    case XCloudClientConfig.FileDownload:
                        Log.Green("Enter remote directory to download a file: ");
                        string remoteFile = Console.ReadLine()!;
                        _socket.Send(Encoding.UTF8.GetBytes(remoteFile));
                    
                        EResponseCode remoteFileStatus = func.ReceiveData(xb.StatusBuffer);
                        if (remoteFileStatus != EResponseCode.FileExists) continue;
                        
                        string remoteFileName = func.ReceiveString(xb.RemoteFileNameBuffer);
                        long remoteFileSize = func.ReceiveLong(new byte[sizeof(long)]);


                        xb.RemoteFileBuffer = new byte[remoteFileSize];
                        _socket.Receive(xb.RemoteFileBuffer);
                        
                        Log.Green("Enter desired directory: ");
                        string localDir = Console.ReadLine()!;

                        if (XCloudClientCore.CreateFile(localDir, remoteFileName, xb.RemoteFileBuffer).Result) {
                            Log.Green("File download success.", true);
                        } else Log.Red("File download unsuccess.", true);
                        break;
                }
            }
        }
    }
}