using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using XCloudClient.Configs;
using XCloudClient.Core;
using XCloudClient.Data;
using XCloudClient.Enums;
using XCloudClient.Internals;
using XCloudClient.Menu;
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
    public void Run() {
        XCloudFunc func = new();
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

                byte[] enterStatusBuffer = new byte[5];
                int enterStatusData = _socket.Receive(enterStatusBuffer);
                string enterStatus = Encoding.UTF8.GetString(enterStatusBuffer, 0, enterStatusData);
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
                        string[]? dirs = func.DeserializeRootDir(buffer, _socket);
                        foreach (var dir in dirs!) {
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

                        _socket.Receive(xb.StatusBuffer);
                        EResponseCode remoteDirStatus = (EResponseCode)BitConverter.ToInt64(xb.StatusBuffer, 0);
                        if (remoteDirStatus == EResponseCode.DirNotExists) {
                            Log.Red($"Remote directory '{remoteDir}' doesn't exist.\nPress any key to continue", true);
                            continue;
                        }

                        Log.Green("Enter file path to upload: ");
                        string myDir = Console.ReadLine()!;

                        try {
                            FileInfo fi = new FileInfo(myDir);

                            if (fi.Exists) _socket.Send(Encoding.UTF8.GetBytes(fi.Name));
                            else {
                                _socket.Send(Encoding.UTF8.GetBytes(XReservedData.InvalidName));
                                Log.Red("File doesn't exist.\nPress any key to continue", true);
                                continue;
                            }
                            _socket.Send(BitConverter.GetBytes(fi.Length));
          
                            _socket.Receive(xb.StatusBuffer);
                            EResponseCode status = (EResponseCode)BitConverter.ToInt64(xb.StatusBuffer, 0);
                            switch (status) {
                                case EResponseCode.FileSizeOk:
                                    _socket.Send(File.ReadAllBytes(myDir));
                                    Log.Green("File upload request sent.\nPress any key to continue", true);
                                    break;
                                case EResponseCode.FileSizeOverflow:
                                    Log.Red("Response: file size overflow.\nPress any key to continue", true);
                                    break;
                            }
                        }
                        catch (Exception ex) {
                            Log.Red($"Error: {ex.Message}");
                        }
                        break;
                }
            }
        }
    }
}