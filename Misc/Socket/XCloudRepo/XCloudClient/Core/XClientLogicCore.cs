using System.Net;
using System.Net.Sockets;
using System.Text;
using XCloudClient.Configs;
using XCloudClient.Enums;
using XCloudClient.Internals;
using XCloudClient.ResponseHandler;

namespace XCloudClient.Core;

public class XClientLogicCore(Socket socket, XCloudFunc func, XBuffer xb, XResponseHandler rh) {
    public async Task<bool> ViewRootDirectoryAsync(byte[] buffer) {
        string[] jsonTree = await func.DeserializeRootDir(buffer);
        if (jsonTree.Length == 0) return false;
        
        foreach (var dir in jsonTree) {
            Log.Blue($"{dir.Replace(@"\", "/")}\n");
        }
        Log.Blue("Enter any key to return false.", true);
        return true;
    }

    public async Task<bool> CreateDirectoryAsync(byte[] buffer) {
        Log.Green("Enter remote directory to create: ");
        string remoteDir = Console.ReadLine()!;
        if (string.IsNullOrEmpty(remoteDir)) return false;
        
        await socket.SendAsync(Encoding.UTF8.GetBytes(remoteDir));
        return true;
    }
    
    public async Task<bool> DeleteDirectoryAsync() {
        Log.Green("Enter remote directory to delete: ");
        string remoteDirToDelete = Console.ReadLine()!;
        if (string.IsNullOrEmpty(remoteDirToDelete)) return false;
        
        await socket.SendAsync(Encoding.UTF8.GetBytes(remoteDirToDelete));
        return true;
    }

    public async Task<bool> RenameDirectoryAsync() {
        Log.Green("Enter current directory name: ");
        string oldDirName = Console.ReadLine()!;
        if (string.IsNullOrEmpty(oldDirName)) return false;
                        
        Log.Green("Enter new directory name: ");
        string newDirName = Console.ReadLine()!;
        if (string.IsNullOrEmpty(newDirName)) return false;
                        
        await socket.SendAsync(Encoding.UTF8.GetBytes(oldDirName));
        await socket.SendAsync(Encoding.UTF8.GetBytes(newDirName));
        return true;
    }
    
    public async Task<bool> UploadFileAsync() {
        Log.Green("Enter remote directory to upload a file: ");
        string remoteDir = Console.ReadLine()!;
        if (string.IsNullOrEmpty(remoteDir)) return false;
                         
        await socket.SendAsync(Encoding.UTF8.GetBytes(remoteDir));

        EResponseCode remoteDirStatus = await func.ReceiveDataAsync(xb.StatusBuffer);
        if (remoteDirStatus == EResponseCode.DirNotExists) return false;

        Log.Green("Enter file path to upload: ");
        string myDir = Console.ReadLine()!;

        try {
            FileInfo fi = new FileInfo(myDir);
            if (!rh.LocalFileExistence(fi.Exists, fi.Name, () => {
                    Log.Red("File doesn't exist\nPress any key to return false", true);
                })) return false;
            socket.Send(BitConverter.GetBytes(fi.Length));
                            
            EResponseCode status = await func.ReceiveDataAsync(xb.StatusBuffer);
            if (!rh.FileSize(status, myDir, () => PLog.FileSize(status))) return false;
        }
        catch (Exception ex) {
            Log.Red($"Error: {ex.Message}");
        }
        return true;
    }
    
    public async Task<bool> DownloadFileAsync() {    
        Log.Green("Enter remote directory to download a file: ");
        string remoteFile = Console.ReadLine()!;
        if (string.IsNullOrEmpty(remoteFile)) return false;

        await socket.SendAsync(Encoding.UTF8.GetBytes(remoteFile));
        EResponseCode remoteFileStatus = await func.ReceiveDataAsync(xb.StatusBuffer);
        if (remoteFileStatus != EResponseCode.FileExists) {
            Log.Red("File not found on server.", true);
            return false;
        }
        
        Log.Green("Enter desired directory: ");
        string localDir = Console.ReadLine()!;
        if (string.IsNullOrEmpty(localDir)) return false;
        
        string remoteFileName = await func.ReceiveStringAsync(xb.RemoteFileNameBuffer);
        long remoteFileSize = await func.ReceiveLongAsync(new byte[sizeof(long)]);
        Log.Red("double async passed");
        remoteFileSize = IPAddress.NetworkToHostOrder(remoteFileSize);

        if (remoteFileSize <= 0) {
            Log.Red($"Invalid file size received: {remoteFileSize}");
            return false;
        }
        Log.Green($"remoteFileSize received ({remoteFileSize} bytes)\n");
        
        string localPath = Path.Combine(localDir, remoteFileName);
        Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);
        
        int n = 0;
        string directory = Path.GetDirectoryName(localPath)!;
        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(localPath);
        string extension = Path.GetExtension(localPath);
        string newFilePath = localPath;

        while (File.Exists(newFilePath)) {
            n++;
            string newFileName = $"{fileNameWithoutExt} ({n}){extension}";
            newFilePath = Path.Combine(directory, newFileName);
        }

        byte[] chunkedBuffer = new byte[XCloudClientConfig.ChunkSize];
        long totalReceived = 0;

        await using (FileStream fs = new FileStream(newFilePath, FileMode.Create, FileAccess.Write)) {
            while (totalReceived < remoteFileSize) {
                int bytesToReceive = (int)Math.Min(XCloudClientConfig.ChunkSize, remoteFileSize - totalReceived);
                int received = await socket.ReceiveAsync(new ArraySegment<byte>(chunkedBuffer, 0, bytesToReceive));

                if (received == 0) {
                    Log.Red("Connection closed unexpectedly.");
                    break;
                }
                
                await fs.WriteAsync(chunkedBuffer.AsMemory(0, received));
                totalReceived += received;
                
                double percent = (double)totalReceived / remoteFileSize * 100;
                Log.Green($"\rReceived: {totalReceived}/{remoteFileSize} bytes ({percent:F1}%)");
            }
        }
        
        EResponseCode finalCode = await func.ReceiveDataAsync(xb.StatusBuffer);
        PLog.FileDownload(finalCode, totalReceived, remoteFileSize);
        return true;
    }

    public async Task<bool> DeleteFileAsync() {
        Log.Green("Enter remote directory to delete a file: ");
        string fileToDelete = Console.ReadLine()!;
        if (string.IsNullOrEmpty(fileToDelete)) return false;
                        
        await socket.SendAsync(Encoding.UTF8.GetBytes(fileToDelete));
        EResponseCode remoteFileToDeleteStatus = await func.ReceiveDataAsync(xb.StatusBuffer);
        PLog.FileDelete(remoteFileToDeleteStatus);
        return true;
    }

    public async Task<bool> RenameFileAsync() {
        Log.Green("Enter remote old directory to rename a file name: ");
        string oldFileName = Console.ReadLine()!;
        if (string.IsNullOrEmpty(oldFileName)) return false;
        
        Log.Green("Enter remote new directory to rename a file name: ");
        string newFileName = Console.ReadLine()!;
        if (string.IsNullOrEmpty(newFileName)) return false;
        
        await socket.SendAsync(Encoding.UTF8.GetBytes(oldFileName));
        EResponseCode oldFileNameStatus = await func.ReceiveDataAsync(xb.StatusBuffer);
        await socket.SendAsync(Encoding.UTF8.GetBytes(newFileName));
        PLog.FileRename(oldFileNameStatus);
        return true;
    }

    public async Task<bool> CopyFileAsync() {
        Log.Green("Enter remote old directory to copy a file name: ");
        string fileNameToCopy = Console.ReadLine()!;
        if (string.IsNullOrEmpty(fileNameToCopy)) return false;
        
        await socket.SendAsync(Encoding.UTF8.GetBytes(fileNameToCopy));
        EResponseCode fileNameToCopyStatus = await func.ReceiveDataAsync(xb.StatusBuffer);
        PLog.FileCopy(fileNameToCopyStatus);
        return true;
    }
}