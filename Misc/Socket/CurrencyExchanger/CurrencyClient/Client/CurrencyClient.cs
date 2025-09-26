using System.Net;
using System.Net.Sockets;
using System.Text;
using CurrencyClient.Client;

public class Client {
    private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private IPEndPoint _ep;

    public Client(string ipPort) {
        _ep = IPEndPoint.Parse(ipPort);
        _socket.Connect(_ep);
    }
    
    public void Run() {
        // if (!CurrencyClientChecker.CheckSemaphore()) {
        //     Log.Red("Server system is currently unavailable.");
        //     return;
        // }
        //
        // if (!CurrencyClientChecker.CheckSocket(_socket)) {
        //     Log.Red("Server is down or busy, try again later.");
        //     return;
        // }
        
        Log.Green("Example: from USD 73 to UAH\n");
        while (true) {
            Log.Green("> ");
            string? request = Console.ReadLine();
            if (request!.Split().Length != 5) {
                Log.Red("Invalid request, please try again.\n");
            }
            
            if (request == "exit") {
                Environment.Exit(0);
                break;
            }
            
            byte[] bytesToSend = Encoding.UTF8.GetBytes(request);
            _socket.Send(bytesToSend);
            
            byte[] responseBuffer = new byte[100];
            int responseData = _socket.Receive(responseBuffer);
            Log.Green(Encoding.UTF8.GetString(responseBuffer, 0, responseData) + "\n");
        }
    }    
}