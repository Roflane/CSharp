using System.Net.Sockets;
using System.Text;

namespace CurrencyClient.Client;

public static class CurrencyClientChecker {
    public static bool CheckSemaphore() {
        if (!Semaphore.TryOpenExisting("CurrencyServer", out Semaphore semaphore)) {
            return false;
        }
        return true;
    }

    public static bool CheckSocket(Socket socket) {
        try {
            byte[] buffer = new byte[100];
            int received = socket.Receive(buffer);
            string msg = Encoding.UTF8.GetString(buffer, 0, received);

            if (msg == "SERVER_BUSY") {
                Log.Red("Server is down or busy, please try again later...");
                return false;
            }
            if (msg == "SERVER_OK") {
                return true;
            }
        }
        catch (Exception) {
            Log.Red("Server is unreachable.");
        }
        return false;
    }

}