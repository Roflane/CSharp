using System.Net;
using System.Net.Sockets;
using RemoteCursor.Winapi;

namespace RemoteCursor.Server;

public class RemoteCursorServer {
    private readonly UdpClient _server;
    private IPEndPoint _ep = new IPEndPoint(IPAddress.Any, 0);

    public RemoteCursorServer(int port) {
        _server = new UdpClient(port);
    }

    public void Run() {
        Console.WriteLine("Server listening...");

        int screenX = WinapiFunc.GetSystemMetrics(NIndex.SM_CXSCREEN);
        int screenY = WinapiFunc.GetSystemMetrics(NIndex.SM_CYSCREEN);

        while (true) {
            byte[] data = _server.Receive(ref _ep);
            int clientX = BitConverter.ToInt32(data, 0);
            int clientY = BitConverter.ToInt32(data, 4);
            
            int finalX = Math.Clamp(clientX, 0, screenX - 1);
            int finalY = Math.Clamp(clientY, 0, screenY - 1);
            
            WinapiFunc.SetCursorPos(finalX, finalY);
            
            byte[] xBytes = BitConverter.GetBytes(screenX);
            byte[] yBytes = BitConverter.GetBytes(screenY);
            byte[] screenData = new byte[xBytes.Length + yBytes.Length];
            Buffer.BlockCopy(xBytes, 0, screenData, 0, xBytes.Length);
            Buffer.BlockCopy(yBytes, 0, screenData, xBytes.Length, yBytes.Length);
            _server.Send(screenData, screenData.Length, _ep);

            Console.Write($"[Server] Client({clientX},{clientY}) → Final({finalX},{finalY}) | Res: {screenX}x{screenY}\r");
        }
    }
}