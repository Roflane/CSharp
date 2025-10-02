using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using XCloudClient.Configs;
using XCloudClient.Data;
using XCloudClient.Enums;
using XCloudClient.Menu;
using XCloudClient.User;

namespace XCloudClient.Client;

public class XCloudClient {
    private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private readonly UserData _userData = new();
    private XCloudData _cloudData = new();
    
    public XCloudClient(string ipPort) {
        var ep = IPEndPoint.Parse(ipPort);
        _socket.Connect(ep);
    }

    [DoesNotReturn]
    public void Run() {
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
                else {
                    Console.Clear();
                    continue;
                }
            }
            else {
                Console.Clear();
                XMenu.PrintCloudOptions();
                
                //_cloudData.TryPrintLastDirs();

                Log.Green("\nEnter option: ");
                string option = Console.ReadLine()!;
                _socket.Send(Encoding.UTF8.GetBytes(option));

                byte[] buffer = new byte[1024 * 10];
                
                switch (option) {
                    case XCloudClientConfig.DirectoryViewRoot: 
                        int dirViewRootData = _socket.Receive(buffer);
                        string[]? dirs = JsonConvert.DeserializeObject<string[]>(Encoding.UTF8.GetString(buffer, 0, dirViewRootData));
                        if (dirs!.Length == 0) {
                            Log.Red("No dirs found.\n");
                            continue;
                        }
                        
                        _cloudData.LastDirs = dirs;
                        foreach (var dir in dirs) {
                            Log.Blue($"{dir.Replace(@"\", "/")}\n");
                        }

                        Console.ReadLine();
                        Log.Blue("Enter any key to skip.");
                        break;
                    case XCloudClientConfig.DirectoryCreate:
                        string newDir = Console.ReadLine()!;
                        _socket.Send(Encoding.UTF8.GetBytes(newDir));
                        break;
                    case XCloudClientConfig.DirectoryDelete:
                        string dirToDelete = Console.ReadLine()!;
                        _socket.Send(Encoding.UTF8.GetBytes(dirToDelete));
                        break;
                    case XCloudClientConfig.DirectoryRename:
                        Log.Green("Enter current directory name: ");
                        string oldDirName = Console.ReadLine()!;
                        
                        Log.Green("Enter new directory name: ");
                        string newDirName = Console.ReadLine()!;
                        
                        _socket.Send(Encoding.UTF8.GetBytes(oldDirName));
                        _socket.Send(Encoding.UTF8.GetBytes(newDirName));
                        break;
                    case XCloudClientConfig.FileUpload:
                        Log.Green("Enter target directory on server: ");
                        string remoteDir = Console.ReadLine()!;
                        _socket.Send(Encoding.UTF8.GetBytes(remoteDir));

                        Log.Green("Enter file path to upload: ");
                        string myDir = Console.ReadLine()!;

                        byte[] statusBuffer = new byte[100];
                        
                        try {
                            FileInfo fi = new FileInfo(myDir);
                            
                            _socket.Send(Encoding.UTF8.GetBytes(fi.Name));
                            _socket.Send(Encoding.UTF8.GetBytes(fi.Length.ToString()));
                            
                            EResponseCode status = (EResponseCode)long.Parse(Encoding.UTF8.GetString(statusBuffer, 0, _socket.Receive(statusBuffer)));
                            switch (status) {
                                case EResponseCode.FileSizeOk:
                                    _socket.Send(File.ReadAllBytes(myDir));
                                    Log.Green("File upload request sent.\nPress any key to continue");
                                    Console.ReadLine();
                                    break;
                                case EResponseCode.FileSizeOverflow:
                                    Log.Red("Response: file size overflow.\nPress any key to continue");
                                    Console.ReadLine();
                                    break;
                            }
                        }
                        catch (Exception ex) {
                            Log.Red($"{ex.Message}\n");
                        }
                        break;
                }
            }
        }
    }
}