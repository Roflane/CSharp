using System.Net.Sockets;
using System.Text;
using CurrencyServer.Helper;

public static class CurrencyServerChecker {
    public static bool InitCheckAndProcess(Semaphore semaphore, Socket client) {
        if (!semaphore.WaitOne(0)) {
            string statusBusy = "SERVER_BUSY";
            client.Send(Encoding.UTF8.GetBytes(statusBusy));
            Logger.Write($"Sent {statusBusy} to client {client.RemoteEndPoint}");
            client.Close();
            return false;
        }
        
        string statusOk = "SERVER_OK";
        client.Send(Encoding.UTF8.GetBytes(statusOk));
        Logger.Write($"Sent {statusOk} to client {client.RemoteEndPoint}");
        return true;
    }
}