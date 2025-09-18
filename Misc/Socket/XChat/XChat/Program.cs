using System.Net;
using System.Net.Sockets;
using System.Text;

namespace XChat;

class Program {
    static void Main() {
        XChatSocket chatSocket = new("127.0.0.1:4773");
        chatSocket.Run();
        
        AutoResetEvent autoEvent = new AutoResetEvent(false);
        autoEvent.WaitOne();
    }
}

static class Log {
    public static void Red(string msg) {
        Console.Write($"\u001b[31m{msg}\u001b[0m");
    }
    
    public static void Green(string msg) {
        Console.Write($"\u001b[32m{msg}\u001b[0m");
    }
    
    public static void Blue(string msg) {
        Console.Write($"\u001b[34m{msg}\u001b[0m");
    }
}

class XChatSocket {
    private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private IPEndPoint _ep;
    
    public XChatSocket(string ipPort) {
        _ep = IPEndPoint.Parse(ipPort);
        _socket.Connect(_ep);
    }
    
    public void Run() {
        bool isAuthorized = false;
        while (!isAuthorized) {
            Log.Green("Input your nickname: ");
            string? nickname = Console.ReadLine();
            if (nickname == "exit") {
                _socket.Close();
                break;
            }

            if (nickname.Length <= 100) {
                byte[] bytesToSend = Encoding.UTF8.GetBytes(nickname);
                _socket.Send(bytesToSend);
            }
            else continue;
            isAuthorized = true;
        }

        if (isAuthorized) {
            Log.Green("Successfully authorized!\n");
        }
        
        while (true) {
            Log.Green("> ");
            string? message = Console.ReadLine();
            if (message == "exit") {
                _socket.Close();
                break;
            }
            
            byte[] bytesToSend = Encoding.UTF8.GetBytes(message);
            _socket.Send(bytesToSend);
        }
    }
}