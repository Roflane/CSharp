using System.Runtime.InteropServices;

class Program {
    private static readonly string _fn = "screen.png";
    
    static void Main() {
        Console.WriteLine("Press 'PrtSc' to take a screenshot.");
        while (true) {
            if (CShot.GetAsyncKeyState(0x2C) != 0) {
                CShot.TakeScreenshot(_fn);
                Console.WriteLine("Screenshot has been saved!");
                Thread.Sleep(800);
            }
            Thread.Sleep(73);
        }
    }
}

public static class CShot {
    [DllImport("CShot.dll")]
    public static extern void TakeScreenshot(string fileName);
    
    [DllImport("User32.dll")]
    public static extern short GetAsyncKeyState(int vk);
}