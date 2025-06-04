using System.Globalization;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Sandbox;

static class Data {
    public static readonly string FileName = "users.json";
    
    public static readonly Dictionary<string, float> Smartphones = new() {
        {"APhone", 28.3f},
        {"BPhone", 74.29f},
        {"CPhone", 43.3f}
    };

    public static readonly Dictionary<string, float> Tablets = new() {
        {"ATablet", 48.3f},
        {"BTablet", 84.29f},
        {"CTablet", 34.3f}
    };

    public static readonly Dictionary<string, float> Laptops = new() {
        {"ALaptop", 120.33f},
        {"BLaptop", 183.29f},
        {"CLaptop", 75.3f}
    };

    public static readonly List<Dictionary<string, float>> Goods = new() {
        Smartphones,
        Tablets,
        Laptops
    };
}

internal static class Program {
    static void Main() {
        LsEmulator();
    }

    static void LsEmulator() {
        GoodsSystem gs = new(Data.Goods);
        
        while (true) {
            Console.Write("Choose one of the following options: Auth (1), Sign up (2): ");
            ELoginStage loginStage = (ELoginStage)Convert.ToSByte(Console.ReadLine());
            
            switch (loginStage) {
                case ELoginStage.Auth:
                    if (gs.Auth()) goto case ELoginStage.Proceed;
                    break;
                case ELoginStage.SignUp:
                    gs.SignUp();
                    break;
                case ELoginStage.Proceed:
                    Console.WriteLine("You are now logged in.\n");
                    Console.WriteLine("Choose one of the following options: Create an order (1), View orders (2): ");
                    
                    byte userChoice = Convert.ToByte(Console.ReadLine());
                    switch (userChoice) {
                        case 1:
                            gs.CreateOrder();
                            break;
                        case 2:
                            gs.ViewOrders();
                            break;
                        default:
                            Console.WriteLine("Invalid choice.\n");
                            continue;
                    }
                        
                    gs.CreateOrder();
                    break;
                default:
                    continue;
            }   
        }
    }
}

enum ELoginStage : sbyte {
    Auth = 1,
    SignUp,
    Proceed
}

enum EGoodsType : sbyte {
    Smartphone = 1,
    Tablet, 
    Laptop
}

enum ESerializationFormat : sbyte {
    Json = 1, 
    Xml,
    Binary
}

struct UserData {
    public string? Username;
    public string? Password;

    public static bool operator==(UserData u1, UserData u2) {
        return u1.Username == u2.Username && u1.Password == u2.Password;;
    }

    public static bool operator!=(UserData u1, UserData u2) {
        return !(u1 == u2);
    }
}

[Serializable]
public struct Order {
    public string? Name;
    public string? Date;
    public float Price;
}

class GoodsSystem {
    private static UserData _user;
    private static List<Dictionary<string, float>>? _goods;
    
    public GoodsSystem(List<Dictionary<string, float>> goods) {
        _goods = goods;
    }
    
    public void SignUp() {
        while (true) {
            UserData ud = new();
            Console.Write("[Sign up] Enter your username: ");
            ud.Username = Console.ReadLine()?.Trim();
            Console.Write("[Sign up] Enter your password: ");
            ud.Password = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(ud.Username) || string.IsNullOrWhiteSpace(ud.Password)) {
                Console.WriteLine("Invalid username or password detected, try again.");
                continue;
            }
            
            List<UserData> users = new List<UserData>();
            if (File.Exists(Data.FileName)) {
                string json = File.ReadAllText(Data.FileName);
                try {
                    users = JsonConvert.DeserializeObject<List<UserData>>(json) ?? new List<UserData>();
                }
                catch (JsonException) {
                    UserData singleUser = JsonConvert.DeserializeObject<UserData>(json);
                    users.Add(singleUser);
                }
            }
            
            if (users.Any(u => u.Username == ud.Username)) {
                Console.WriteLine("The login already exists!");
                continue;
            }
            
            users.Add(ud);
            
            File.WriteAllText(Data.FileName, JsonConvert.SerializeObject(users, Formatting.Indented));
            Console.WriteLine($"Successful sign-up, {ud.Username}!");
            break;
        }
    }

    public bool Auth() {
        while (true) {
            UserData ud = new();
            Console.Write("[Auth] Enter your username: ");
            ud.Username = Console.ReadLine();
            Console.Write("[Auth] Enter your password: ");
            ud.Password = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(ud.Username) || string.IsNullOrWhiteSpace(ud.Password)) {
                Console.WriteLine("Invalid username or password detected, try again.");
                continue;
            }
            
            List<UserData> users = new List<UserData>();
            if (File.Exists(Data.FileName)) {
                string json = File.ReadAllText(Data.FileName);
                try {
                    users = JsonConvert.DeserializeObject<List<UserData>>(json) ?? new List<UserData>();
                }
                catch (JsonException) {
                    UserData singleUser = JsonConvert.DeserializeObject<UserData>(json);
                    users.Add(singleUser);
                }
            }

            if (users.Any(u =>
                    u.Username == ud.Username &&
                    u.Password == ud.Password)) {
                _user.Username =  ud.Username;
                _user.Password =  ud.Password;
                return true;
            }
            return false;
        }
    }

    public void CreateOrder() {
        while (true) {
            Console.Write("Choose one of the following goods: Smartphone (1), Tablet (2), Laptop (3): ");
            EGoodsType goodsType = (EGoodsType)Convert.ToInt32(Console.ReadLine());
        
            Order order = new();
        
            switch (goodsType) {
                case EGoodsType.Smartphone:
                    Console.WriteLine("--Available smartphones--");
                    foreach (var smartphone in Data.Smartphones) {
                        Console.WriteLine($"Name: {smartphone.Key} | Price: {smartphone.Value}");
                    }
                    Console.Write("\nEnter desired smartphone name: ");
                    
                    order.Name = Console.ReadLine();
                    if (!Data.Smartphones.ContainsKey(order.Name)) {
                        Console.WriteLine("Invalid smartphone name detected, try again.");
                        continue;
                    }
                    order.Price = Data.Smartphones[order.Name];
                    break;
                case EGoodsType.Tablet:
                    Console.WriteLine("--Available tablets--");
                    foreach (var tablet in Data.Tablets) {
                        Console.WriteLine($"Name: {tablet.Key} | Price: {tablet.Value}");
                    }
                    Console.Write("\nEnter desired tablet name: ");
                    
                    order.Name = Console.ReadLine();
                    if (!Data.Tablets.ContainsKey(order.Name)) {
                        Console.WriteLine("Invalid tablet name detected, try again.");
                        continue;
                    }
                    order.Price = Data.Tablets[order.Name];
                    break;
                case EGoodsType.Laptop:
                    Console.WriteLine("--Available laptops--");
                    foreach (var laptop in Data.Laptops) {
                        Console.WriteLine($"Name: {laptop.Key} | Price: {laptop.Value}");
                    }
                    Console.Write("\nEnter desired laptop name: ");
                    
                    order.Name = Console.ReadLine();
                    if (!Data.Laptops.ContainsKey(order.Name!)) {
                        Console.WriteLine("Invalid laptop name detected, try again.");
                        continue;
                    }
                    order.Price = Data.Laptops[order.Name];
                    break;
            }
            
            order.Date = DateTime.Now.ToString(CultureInfo.InvariantCulture);

            string path = $"data/{_user.Username}";
            if (!Path.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            Console.Write("Enter one of the following serialization methods: Json [Recommended] (1), XML (2), Binary (3): ");
            ESerializationFormat format = (ESerializationFormat)Convert.ToByte(Console.ReadLine());
            List<Order> orders = new();
            
            switch (format) {
                case ESerializationFormat.Json:
                    string jsonPath = $"{path}/orders.json";
                    
                    if (File.Exists(jsonPath)) {
                        string existingJson = File.ReadAllText(jsonPath);
                        orders = JsonConvert.DeserializeObject<List<Order>>(existingJson) ?? new List<Order>();
                    }
                    
                    orders.Add(order);
                    File.WriteAllText(jsonPath, JsonConvert.SerializeObject(orders, Formatting.Indented));
                    break;
                case ESerializationFormat.Xml:
                    string xmlPath = $"{path}/orders.xml";
                    
                    if (File.Exists(xmlPath) && new FileInfo(xmlPath).Length > 0) {
                        var serializerRead = new XmlSerializer(typeof(List<Order>));
                        using var reader = new StreamReader(xmlPath);
                        orders = (List<Order>)serializerRead.Deserialize(reader)! ?? new List<Order>();
                    }
                    
                    orders.Add(order);
                    var serializerWrite = new XmlSerializer(typeof(List<Order>));
                    using (var writer = new StreamWriter(xmlPath))
                        serializerWrite.Serialize(writer, orders);
                    break;
                case ESerializationFormat.Binary:
                    using (var fs = new FileStream($"{path}/orders.bin", FileMode.Append, FileAccess.Write))
                    using (var writer = new BinaryWriter(fs)) {
                        writer.Write(order.Name!);
                        writer.Write(order.Date);
                        writer.Write(order.Price);
                    }
                    break;
                default:
                    Console.WriteLine("Enter valid serialization format!");
                    continue;
            }
            Console.WriteLine("Order log created!");
            return;
        }
    }

    public void ViewOrders() {
        string path = $"data/{_user.Username}";
        string jsonPath = $"{path}/orders.json";
        string binPath = $"{path}/orders.bin";
        
        Console.WriteLine("Choose one of the following view options: Json (1), Bin (2)");
        sbyte userChoice = Convert.ToSByte(Console.ReadLine());

        switch (userChoice) {
            case 1:
                if (File.Exists(jsonPath)) {
                    string existingJson = File.ReadAllText(jsonPath);
                    var orders = JsonConvert.DeserializeObject<List<Order>>(existingJson) ?? new List<Order>();

                    foreach (var order in orders) {
                        Console.WriteLine($"{order.Name} -  {order.Price} - {order.Date}");
                    }
                }
                break;
            case 2:
                using (var fs = new FileStream(binPath, FileMode.Open, FileAccess.Read))
                using (var reader = new BinaryReader(fs)) {
                    while (fs.Position < fs.Length) {
  
                        string name = reader.ReadString();
                        string date = reader.ReadString();
                        float price = reader.ReadSingle();
                        Console.WriteLine($"{name} - {price} - {date}");
                    }
                }
                break;
        }
        
     
    }
}
