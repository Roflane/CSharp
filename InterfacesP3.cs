using System.Globalization;
using System.Text.Json;

namespace Practice;

internal static class Program {
    static void Main() {
        WeatherInfo();
    }

    static void WeatherInfo() {
        WeatherInfo wi = new("https://pastebin.com/raw/nwBVQ02j");

        while (true) {
            Console.Write("Enter a city name: ");
            string? city = Console.ReadLine();
            string?[] info = wi.GetWeatherInfo(city!);
            wi.PrintInfo(info);
            Console.WriteLine();
        }
    }
}

interface IWeatherInfo {
    public string GetKeyByRequest(string url);
    public void PrintInfo(string?[] weatherInfo);
}

class WeatherInfo : IWeatherInfo {
    private readonly string _apiKey;
    
    public WeatherInfo(string url) {
        _apiKey = GetKeyByRequest(url);
    }
    
    // Methods
    public string GetKeyByRequest(string url) {
        using HttpClient receiver = new HttpClient();
        HttpResponseMessage keyResponse = new();

        try {
            keyResponse = receiver.GetAsync(url).Result;
        }
        catch (AggregateException e) {
            Console.WriteLine(e.Message);
        }
        
        string apiKey = keyResponse.Content.ReadAsStringAsync().Result;
        return apiKey;
    }
    
    public string?[] GetWeatherInfo(string city) {
        using HttpClient httpClient = new();
        httpClient.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
        
        string url = $"weather?q={city}&appid={_apiKey}&units=metric";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        HttpResponseMessage response = httpClient.Send(request);
        
        string json = response.Content.ReadAsStringAsync().Result;
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        string? cityString = null;
        string? descString =  null;
        float temperature = -99999;
        
        try {
            cityString = root.GetProperty("name").GetString();
            descString = root.GetProperty("weather")[0].GetProperty("description").GetString();
            temperature = root.GetProperty("main").GetProperty("temp").GetSingle();
        }
        catch (KeyNotFoundException e) {
            Console.WriteLine(e.Message);
        }
        
        return [cityString, descString, temperature.ToString(CultureInfo.InvariantCulture)];
    }

    public void PrintInfo(string?[] weatherInfo) {
        if (weatherInfo[0] == null || weatherInfo[1] == null || 
            weatherInfo[2] == "-99999") return;
        Console.WriteLine($"""
                           City: {weatherInfo[0]},
                           Description: {weatherInfo[1]},
                           Temperature: {weatherInfo[2]} °C,
                           """);
    }
}
