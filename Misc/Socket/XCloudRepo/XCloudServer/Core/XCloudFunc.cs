using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace XCloudRepo.Core;

public class XCloudFunc {
    public int SerializeRootDir(Socket client, XCloudCore core) {
        string json = JsonConvert.SerializeObject(core.DirectoryViewRoot());
        return client.Send(Encoding.UTF8.GetBytes(json));
    }
    
    public string ReceiveString(Socket client, byte[] buffer) {
        return Encoding.UTF8.GetString(buffer, 0, client.Receive(buffer));
    }
}