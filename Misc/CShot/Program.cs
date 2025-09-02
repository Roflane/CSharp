using System.Runtime.InteropServices;

class Program {
    static void Main() {
        Console.WriteLine("Press 'PrtSc' to take a screenshot.");
        while (true) {
            if (CShot.GetAsyncKeyState(0x2C) != 0) {
                CShot.TakeScreenshot();
                Console.WriteLine($"[{DateTime.Now}] Screenshot has been saved!");
                Thread.Sleep(800);
            }
            Thread.Sleep(73);
        }
    }
}

public static class CShot {
    [DllImport("CShot.dll")]
    public static extern void TakeScreenshot();
    
    [DllImport("User32.dll")]
    public static extern short GetAsyncKeyState(int vk);
}
