using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace XCloudClient.Core;

public class XCloudFunc {
    public string[]? DeserializeRootDir(byte[] buffer, Socket socket) {
        return JsonConvert.DeserializeObject<string[]>(Encoding.UTF8.GetString(buffer, 0, socket.Receive(buffer)));
    }
}