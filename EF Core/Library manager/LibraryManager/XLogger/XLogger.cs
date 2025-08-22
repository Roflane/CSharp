using System.IO;

public class XLogger {
    private readonly string _fileName;
    
    public XLogger(string fileName) {
        _fileName = fileName;
    }
    
    public void LogToFile(string msg) {
        using FileStream fs = new(_fileName, FileMode.Append, FileAccess.Write);
        using StreamWriter sw = new(fs);
        sw.WriteLine($"[{DateTime.Now}] {msg}");
    }
}