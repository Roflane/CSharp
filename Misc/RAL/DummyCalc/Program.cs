using System.Reflection;
using System.Runtime.Loader;

namespace DummyCalc;

class Program {
    static void Main() {
        string currentDir = Directory.GetCurrentDirectory() + "\\Calc.dll";
        var assembly = Assembly.LoadFile(currentDir);
        var type = assembly.GetType("Calc.CalcCore");
        var method = type.GetMethods().First(s => s.Name == "Div");
        var res = method.Invoke(null, [new double[] { 1, 2, 3, 4 }]);
        Console.WriteLine(res);
    }
}