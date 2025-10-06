using System.Net.Sockets;
using XCloudRepo.Configs;
using XCloudRepo.Enums;

namespace XCloudRepo.ResponseHandler;

public class XResponseHandler {
    public bool DirectoryExistence(XCloudCore core, string dir, Socket client, Action log) {
        if (!core.DirectoryExists(dir)) {
            log.Invoke();
            client.Send(BitConverter.GetBytes((long)EResponseCode.DirNotExists));
            return false;
        }
        client.Send(BitConverter.GetBytes((long)EResponseCode.DirExists));
        return true;
    }
    
    public bool FileSize(long fileSize, Socket client, Action log) {
        if (fileSize > XCloudServerConfig.MaxFileBufferSize) {
            log.Invoke();
            client.Send(BitConverter.GetBytes((long)EResponseCode.FileSizeOverflow));
            return false;
        }
        client.Send(BitConverter.GetBytes((long)EResponseCode.FileSizeOk));
        return true;
    }

    public bool FileExistence(string file, Socket client, Action log) {
        if (!File.Exists(file)) {
            log.Invoke();
            client.Send(BitConverter.GetBytes((long)EResponseCode.FileNotExists));
            return false;
        } 
        client.Send(BitConverter.GetBytes((long)EResponseCode.FileExists));
        return true;
    }
    
    public async Task<bool> LocalFileExistsAsync(string file, Socket client, Action log) {
        if (!File.Exists(file)) {
            log.Invoke();
            byte[] response = BitConverter.GetBytes((long)EResponseCode.FileNotExists);
            await client.SendAsync(response);
            return false;
        }
        byte[] existsResponse = BitConverter.GetBytes((long)EResponseCode.FileExists);
        await client.SendAsync(existsResponse);
        return true;
    }
    
    // public bool FileName(string fileName, Socket client, Action log) {
    //     if (fileName == XReservedData.InvalidName) {
    //         log.Invoke();
    //         client.Send(BitConverter.GetBytes((long)EResponseCode.FileNotExists));
    //         return false;
    //     }
    //     client.Send(BitConverter.GetBytes((long)EResponseCode.FileExists));
    //     return true;
    // }
}