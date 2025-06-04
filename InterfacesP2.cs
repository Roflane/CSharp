namespace Practice;

internal static class Program {
    static void Main()
    {
        Task1();
        Task2();
        Task3();
        Task4();
        Task5();
    }

    static void Task1() {
        Stack stack = new();
        
        stack.Push(1);  
        stack.Push(2);
        stack.Push(73);

        stack.Print(); 
        Console.WriteLine();

        Int32 targetValue = 70;
        Console.WriteLine($"Less than {targetValue}: {stack.Less(targetValue)}");
        Console.WriteLine($"Greater than {targetValue}: {stack.Greater(targetValue)}");
    }

    static void Task2()
    {
        Stack stack = new();
        
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        stack.Push(4);
        stack.Push(5);
        stack.Push(6);
        
        stack.ShowEven();
        stack.ShowOdd();
    }

    static void Task3() {
        Stack stack = new();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        
        Console.WriteLine($"Distinct values: {stack.CountDistinct()}");
        Console.WriteLine($"Equal values to: {stack.EqualToValue(3)}");
    }

    static void Task4() {
        TVEmulator tv = new();
        tv.SetChannel(1);
        tv.PrintCurrentChannel();
        tv.SetChannel(2);
        tv.PrintCurrentChannel();
    }

    static void Task5() {
        UserData ud = new("xd", "qwerty");
        Console.WriteLine(ud.Login);
        Console.WriteLine(ud.Password);
    }
}

interface ICalc {
    public Int32 Less(Int32 targetValue);
    public Int32 Greater(Int32 targetValue);
}

interface ICalc2 {
    public Int32 CountDistinct();
    public Int32 EqualToValue(Int32 targetValue);
}

interface IOutput {
    void ShowEven();
    void ShowOdd();
}

interface IRemoteControl {
    void TurnOn();
    void TurnOff();
    void SetChannel(byte channel);
}

interface IValidator {
    bool Validate(string? login, string password);
}

class Stack : ICalc, ICalc2, IOutput {
    // Fields
    private List<Int32> _stack;

    public Stack() {
        _stack = new List<Int32>();
    }

    // Methods
    public List<Int32> GetStack() => _stack;
    public long Size() => _stack.Count;
    public bool Empty() => _stack.Count == 0;
    public Int32 Top() => _stack.Count - 1;
    
    public void Push(Int32 value) => _stack.Add(value);
    
    public Int32 Pop() {
        Int32 temp = _stack[^1];
        _stack.RemoveAt(_stack.Count - 1);
        return temp;
    }

    // ICalc
    public Int32 Less(Int32 targetValue) {
        Int32 count = 0;
        foreach (Int32 value in _stack) {
            if (value < targetValue) count++;
        }
        return count;
    }

    public Int32 Greater(Int32 targetValue) {
        Int32 count = 0;
        foreach (Int32 value in _stack) {
            if (value > targetValue) count++;
        }
        return count - 1;
    }
    
    // ICalc2
    public Int32 CountDistinct() {
        Int32 count = 0;
        bool isUnique = false;
        for (int i = 1; i < _stack.Count; ++i) {
            if (_stack[i] != _stack[i - 1]) {
                isUnique = true;
            }
            count++;
        }
        return count;
    }

    public Int32 EqualToValue(Int32 targetValue) {
        Int32 count = 0;
        foreach (Int32 value in _stack) {
            if (value == targetValue) {
                ++count;
            }
        }
        return count;
    }
    
    // IOutput
    public void ShowEven() {
        Console.Write("Even: ");
        foreach (Int32 value in _stack) {
            if (value % 2 == 0) Console.Write($"{value} ");
        }
        Console.WriteLine();
    }

    public void ShowOdd() {
        Console.Write("Odd: ");
        foreach (Int32 value in _stack) {
            if (value % 2 != 0) Console.Write($"{value} ");
        }
        Console.WriteLine();
    }

    public void Print() {
        foreach (Int32 i in _stack) Console.Write(i + " ");
    } 
}

class TVEmulator : IRemoteControl {
    private byte _channelCount;
    private List<byte> _channels;
    private byte _currentChannel;

    public TVEmulator() {
        _channelCount = 16;
        _channels = new List<byte>();
        _currentChannel = 1;
    }
    
    // Methods
    public void TurnOn() {
        Console.WriteLine("O");   
    }

    public void TurnOff() {
        Console.WriteLine("X");
    }

    public void SetChannel(byte channel) {
        _currentChannel = channel;
    }

    public void PrintCurrentChannel() {
        Console.WriteLine($"{_currentChannel} ");
    }
}

class UserData : IValidator {
    private string _login;
    private string _password;
    
    public string Login => _login;
    public string Password => _password;
    
    public UserData(string login, string password) {
        if (Validate(login, password))
        {
            _login = login;
            _password = password;
        }
    }

    public bool Validate(string? login, string? password) {
        if (login != null && password != null) return true;
        else return false;
    }
}
