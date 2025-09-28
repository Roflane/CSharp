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
        if (!CurrencyClientChecker.CheckSemaphore()) {
            Log.Red("Currency system is currently unavailable.\n");
            return;
        }
        
        if (!CurrencyClientChecker.CheckSocket(_socket)) {
            return;
        }
        
        Log.Blue("Example: from USD 73 to UAH\n");
        while (_socket.Connected) {
            Log.Green("\n> ");
            string? request = Console.ReadLine();
            if (request == "exit") {
                _socket.Close();
                break;
            }

            if (request!.Contains(",")) {
                request = request.Replace(",", ".");
            }
            
            byte[] bytesToSend = Encoding.UTF8.GetBytes(request);
            _socket.Send(bytesToSend);
            
            byte[] responseBuffer = new byte[1024];
            int responseData = _socket.Receive(responseBuffer);
            string responseString = Encoding.UTF8.GetString(responseBuffer, 0, responseData);
            if (responseString.Contains("Invalid")) {
                Log.Red($"Negative response got: {responseString}");
            }
            else {
                Log.Green($"Response received: {responseString}");
            }
        }
    }    
}