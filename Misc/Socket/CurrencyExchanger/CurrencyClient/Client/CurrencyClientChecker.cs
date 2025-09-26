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
            socket.Send(Encoding.UTF8.GetBytes("STATUS_SUCCESS"));
        }
        catch (Exception) {
            Log.Red("Server is busy, please try again later...");
            return false;
        }
        return true;
    }
}