using System.Net.Sockets;
using System.Text;
using XCloudClient.Configs;
using XCloudClient.Enums;

namespace XCloudClient.ResponseHandler;

public class XResponseHandler(Socket socket) {
    public bool RemoteDir(EResponseCode status, Action log) {
        if (status == EResponseCode.DirNotExists) {
            socket.Send(BitConverter.GetBytes((long)EResponseCode.DirNotExists));
            log.Invoke();
            return false;
        }
        socket.Send(BitConverter.GetBytes((long)EResponseCode.DirNotExists));
        return true;
    }
    
    public bool LocalFileExistence(bool status, string name, Action log) {
        if (!status)  {
            socket.Send(Encoding.UTF8.GetBytes(XReservedData.InvalidName));
            log.Invoke();
            return false;
        }
        socket.Send(Encoding.UTF8.GetBytes(name));
        return true;
    }

    public bool FileSize(EResponseCode status, string fileDir, Action log) {
        switch (status) {
            case EResponseCode.FileSizeOk:
                socket.Send(File.ReadAllBytesAsync(fileDir).Result);
                return false;
        } 
        log.Invoke();
        return true;
    }
    
    
}