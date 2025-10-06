using System.Net;
using System.Net.Sockets;
using System.Text;
using XCloudRepo.Configs;
using XCloudRepo.Enums;
using XCloudRepo.Internals;
using XCloudRepo.ResponseHandler;

namespace XCloudRepo.Core;

public class XServerLogicCore(Socket client, XCloudFunc func, XBuffer xb, XResponseHandler rh) {
    public bool ViewRootDirectory(XCloudCore core) {
        bool jsonTreeStatus = func.SerializeRootDir(client, core) > 0;
        if (!jsonTreeStatus) return false;
        
        PLog.DirectoryViewRoot(jsonTreeStatus, 
            client.RemoteEndPoint!.ToString());
        return true;
    }

    public async Task<bool> CreateDirectoryAsync(XCloudCore core) {
        string dirToCreate = await func.ReceiveStringAsync(client, xb.DirToCreateBuffer);
        if (string.IsNullOrEmpty(dirToCreate)) return false;
        
        PLog.DirectoryCreate(core.DirectoryCreate(dirToCreate), 
            client.RemoteEndPoint!.ToString(), dirToCreate);
        return true;
    }
    
    public async Task<bool> DeleteDirectoryAsync(XCloudCore core) {
        string dirDelete = await func.ReceiveStringAsync(client, xb.DirToDeleteBuffer);
        if (string.IsNullOrEmpty(dirDelete)) return false;
        
        PLog.DirectoryDelete(core.DirectoryDelete(dirDelete), 
            client.RemoteEndPoint!.ToString(), dirDelete);
        return true;
    }

    public async Task<bool> RenameDirectoryAsync(XCloudCore core) {
        string oldDir = await func.ReceiveStringAsync(client, xb.OldDirBuffer);
        string newDir = await func.ReceiveStringAsync(client, xb.NewDirBuffer);
        if (string.IsNullOrEmpty(oldDir) || string.IsNullOrEmpty(newDir)) return false;
        
        PLog.DirectoryRename(core.DirectoryRename(oldDir, newDir), client.RemoteEndPoint!.ToString(), oldDir, newDir);
        return true;
    }

    public async Task<bool> UploadFileAsync(XCloudCore core) {
        string dirToUpload = await func.ReceiveStringAsync(client, xb.DirToUploadBuffer);
        if (!rh.DirectoryExistence(core, dirToUpload, client,
                () => { Log.Red("Directory doesn't exist."); })) {
            return false;
        }
                            
        string fileName = await func.ReceiveStringAsync(client, xb.FileNameBuffer);
        if (fileName == XReservedData.InvalidName) return false;
                            
        long fileSize = func.ReceiveLong(client, xb.FileSizeBuffer);
        if (!rh.FileSize(fileSize, client,
                () => { Log.Red("File size overflow."); })) {
            return false;
        }

        xb.FileToUploadBuffer = new byte[fileSize];
        client.Receive(xb.FileToUploadBuffer);
        PLog.FileUpload(core.FileUpload(dirToUpload, fileName, xb.FileToUploadBuffer).Result, 
            client.RemoteEndPoint!.ToString(), fileName);
        return true;
    }

    public async Task<bool> DownloadFileAsync(XCloudCore core) {
        string remotePath = await func.ReceiveStringAsync(client, xb.FileToDownloadBuffer);
        string fileToDownload = $"{core.RootDir}/{remotePath}";
        Log.Red(fileToDownload);
        if (!await rh.LocalFileExistsAsync(remotePath, client, () => Log.Red("File doesn't exist."))) {
            return false;
        }

        FileInfo fi = new FileInfo(fileToDownload);
        await client.SendAsync(Encoding.UTF8.GetBytes(fi.Name));
        await client.SendAsync(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(fi.Length)));
        Log.Red("double async passed");
        byte[] buffer = new byte[XCloudServerConfig.chunkSize];
        long totalSent = 0;

        await using (FileStream fs = new FileStream(fileToDownload, FileMode.Open, FileAccess.Read, FileShare.Read)) {
            int bytesRead;
            while ((bytesRead = await fs.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0) {
                await client.SendAsync(new ArraySegment<byte>(buffer, 0, bytesRead));
                totalSent += bytesRead;
            }
        }

        Log.Green($"File download done ({fi.Name}, {fi.Length} bytes)");
        await client.SendAsync(BitConverter.GetBytes((long)EResponseCode.FileTransferComplete));
        return true;
    }

    public async Task<bool> DeleteFileAsync(XCloudCore core) {
        string fileToDelete = $"{core.RootDir}/{await func.ReceiveStringAsync(client, xb.FileToDownloadBuffer)}";
        if (!await rh.LocalFileExistsAsync(fileToDelete, client, () =>  { Log.Red("File doesn't exist."); })) {
            return false;
        }

        if (await core.FileDelete(fileToDelete)) {
            Log.Green("Request 'FileDelete' succeeded");
        } else Log.Green("Request 'FileDelete' unsucceeded");
        return true;
    }

    public async Task<bool> RenameFileAsync(XCloudCore core) {
        string oldFileName = $"{core.RootDir}/{await func.ReceiveStringAsync(client, xb.FileToRenameBuffer)}";
        if (!await rh.LocalFileExistsAsync(oldFileName, client, () =>  { Log.Red("File doesn't exist."); })) {
            return false;
        }
        string newFileName = $"{core.RootDir}/{await func.ReceiveStringAsync(client, xb.FileToRenameBuffer)}";
                            
        
        Log.Red(oldFileName);
        Log.Red(newFileName);
                            
        if (await core.FileRename(oldFileName, newFileName)) {
            Log.Green("Request 'FileRename' succeeded.");
        } else Log.Red("Request 'FileRename' unsucceeded.");
        return true;
    }

    public async Task<bool> CopyFileAsync(XCloudCore core) {
        string fileToCopy = $"{core.RootDir}/{await func.ReceiveStringAsync(client, xb.FileToCopyBuffer)}";
        if (!await rh.LocalFileExistsAsync(fileToCopy, client, () =>  { Log.Red("File doesn't exist."); })) {
            return false;
        }
                            
        if (await core.FileCopy(fileToCopy)) {
            Log.Green("Request 'FileCopy' succeeded.");
        } else Log.Green("Request 'FileCopy' unsucceeded.");
        return true;
    }
}