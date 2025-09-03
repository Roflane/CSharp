namespace ThreadSecond;

class Program {
    private static int _counter;
    static void Main() {
        var mutex = Mutex.OpenExisting("SharedThread");
        ThreadSecond(mutex, ref _counter);
    }

    static void ThreadSecond(Mutex mutex, ref int counter) {
        while (true) {
            mutex.WaitOne();
            counter += 2;
            Console.WriteLine("SecondThread: " + counter);
            Thread.Sleep(500);
            mutex.ReleaseMutex();
        }
    }
}