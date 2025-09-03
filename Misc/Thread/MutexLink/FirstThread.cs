namespace ThreadFirst;

class Program {
    private static int _counter;
    static void Main() {
        var mutex = new Mutex(false, "SharedThread");
        ThreadFirst(mutex, ref _counter);
    }

    static void ThreadFirst(Mutex mutex, ref int counter) {
        while (true) {
            mutex.WaitOne();
            counter++;
            Console.WriteLine("FirstThread: " + counter);
            Thread.Sleep(250);
            mutex.ReleaseMutex();
        }
    }
}