namespace Sandbox;

internal static class Program {
    private static readonly List<Action> SensorEvents = [
        Sensor.OnValueIncreased,
        Sensor.OnValueDecreased,
        Sensor.OnMaxValue,
        Sensor.OnValueSet
    ];
    
    static void Main() {
        SubscriberManager sm = new(1, SensorEvents);
        SubscriberManager_Start(sm);
    }

    static void SubscriberManager_Start(SubscriberManager sm) {
        Console.WriteLine("""
                          1. Add a subscriber
                          2. Remove a subscriber
                          3. Set amount of subscribers
                          4. Show amount of subscribers
                          5. Exit
                          """);
        
        while (true) {
            Console.Write("> ");
            byte userInput = Convert.ToByte(Console.ReadLine());
                        
            switch (userInput) {
                case 1:
                    sm.AddSubscriber();
                    break;
                case 2:
                    sm.RemoveSubscriber();
                    break;
                case 3:
                    int subscriberAmount = Convert.ToInt32(Console.ReadLine());
                    sm.SetSubscribers(subscriberAmount);
                    break;
                case 4:
                    Console.WriteLine(sm.GetSubscribers());
                    break;
                case 5: return;
            }
            sm.CheckSubscriberTreshold();
        }
    }
}

enum ESensorEvents : byte {
    ValueIncreased,
    ValueDecreased, 
    MaxValue
}

public static class Sensor {
    public static void OnValueIncreased() {
        Console.WriteLine("[LOG]: Value increased");
    }
    
    public static void OnValueDecreased() {
        Console.WriteLine("[LOG]: Value decreased");
    }

    public static void OnMaxValue() {
        Console.WriteLine("[LOG]: Value reached max value");
    }

    public static void OnValueSet() {
        Console.WriteLine("[LOG]: Value is overwritten");
    }
}

class SubscriberManager {
    // Fields
    private int _subscribers;
    private readonly List<Action> _sensors;
    
    // Properties
    public int GetSubscribers() => _subscribers;

    public void SetSubscribers(int value) {
        if (value < 0) return;
        _subscribers = value;
    } 
    
    public SubscriberManager(int initSubscribers, List<Action> sensors) {
        _subscribers = initSubscribers;
        _sensors = sensors;
    }

    // Methods
    public void CheckSubscriberTreshold() {
        if (_subscribers == 1000) {
            _subscribers = 0;
            _sensors[(byte)ESensorEvents.MaxValue].Invoke();
        }
    }
    
    public void AddSubscriber() {
        ++_subscribers;
        _sensors[(byte)ESensorEvents.ValueIncreased].Invoke();
    }
    
    public void RemoveSubscriber() {
        --_subscribers;
        _sensors[(byte)ESensorEvents.ValueDecreased].Invoke();
    }
}
