using System.Net.Sockets;
using XCloudRepo.Configs;
using XCloudRepo.Enums;

namespace XCloudRepo.ResponseHandler;

public class XResponseHandler {
    public bool DirectoryExistance(XCloudCore core, string dir, Socket client, Action log) {
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
}