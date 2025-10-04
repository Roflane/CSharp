using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using XCloudClient.Enums;
using XCloudClient.Internals;

namespace XCloudClient.Core;

public class XCloudFunc(Socket socket) {
    public string[]? DeserializeRootDir(byte[] buffer) {
        return JsonConvert.DeserializeObject<string[]>(Encoding.UTF8.GetString(buffer, 0, socket.Receive(buffer)));
    }

    public string ReceiveString(byte[] buffer) {
        return Encoding.UTF8.GetString(buffer, 0, socket.Receive(buffer));
    }
    
    public EResponseCode ReceiveData(byte[] buffer) {
        socket.Receive(buffer);
        return (EResponseCode)BitConverter.ToInt64(buffer, 0);
    }
    
    public long ReceiveLong(byte[] buffer) {
        socket.Receive(buffer);
        return BitConverter.ToInt64(buffer, 0);
    }
}