using System.Net.Sockets;
using System.Text;
using XCloudClient.Configs;
using XCloudClient.Enums;

namespace XCloudClient.ResponseHandler;

public class XResponseHandler(Socket socket) {
    public bool RemoteDir(EResponseCode status, Action log) {
        if (status == EResponseCode.DirNotExists) {
            log.Invoke();
            return false;
        }
        return true;
    }
    
    public bool LocalFileExistence(bool status, string name, Action log) {
        if (!status)  {
            socket.SendAsync(Encoding.UTF8.GetBytes(XReservedData.InvalidName));
            log.Invoke();
            return false;
        }
        socket.SendAsync(Encoding.UTF8.GetBytes(name));
        return true;
    }

    public bool FileSize(EResponseCode status, string fileDir, Action log) {
        switch (status) {
            case EResponseCode.FileSizeOk:
                socket.SendAsync(File.ReadAllBytesAsync(fileDir).Result);
                return false;
        } 
        log.Invoke();
        return true;
    }
}