using System.Runtime.InteropServices;
using RemoteCursor.Winapi;

public static class WinapiFunc {
    [DllImport("User32.dll")]
    public static extern int GetSystemMetrics(NIndex nIndex);
    
    [DllImport("User32.dll")]
    public static extern int GetCursorPos(out tagPOINT lpPoint);
    
    [DllImport("User32.dll")]
    public static extern int SetCursorPos(int x, int y);
}