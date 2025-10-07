using System.Net;
using System.Net.Sockets;
using RemoteCursor.Winapi;

namespace RemoteCursor.Client;

public class RemoteCursorClient(string ipPort) {
    private readonly UdpClient _client = new();
    private IPEndPoint _ep = IPEndPoint.Parse(ipPort);
    
    public void Run() {
        Console.WriteLine("Client running...");

        while (true) {
            int myX = WinapiFunc.GetSystemMetrics(NIndex.SM_CXSCREEN);
            int myY = WinapiFunc.GetSystemMetrics(NIndex.SM_CYSCREEN);
            
            WinapiFunc.GetCursorPos(out tagPOINT p);
            byte[] xBytes = BitConverter.GetBytes(p.x);
            byte[] yBytes = BitConverter.GetBytes(p.y);
            byte[] data = new byte[xBytes.Length + yBytes.Length];
            Buffer.BlockCopy(xBytes, 0, data, 0, xBytes.Length);
            Buffer.BlockCopy(yBytes, 0, data, xBytes.Length, yBytes.Length);
            _client.Send(data, data.Length, _ep);
            
            byte[] serverScreenData = _client.Receive(ref _ep);
            int serverX = BitConverter.ToInt32(serverScreenData, 0);
            int serverY = BitConverter.ToInt32(serverScreenData, sizeof(int));
            
            int normalizedX = (int)((float)p.x / myX * serverX);
            int normalizedY = (int)((float)p.y / myY * serverY);

            Console.Write($"[Client] Local({p.x},{p.y}) → Server({normalizedX},{normalizedY}) | Res {myX}x{myY} → {serverX}x{serverY}\r");

            Thread.Sleep(10);
        }
    }
}
