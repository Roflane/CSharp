using System.Text;
using System.Text.Json;

namespace TodoWeb;

public static class Program {
    public static async Task Main(string[] args) {
        Console.OutputEncoding = Encoding.UTF8;

        if (args.Length < 1) {
            Console.WriteLine(
                @"Usage:
                  TodoClient add <title> <description>
                  TodoClient list
                  TodoClient get <id>
                  TodoClient delete <id>
                  TodoClient update <id> <title> <description>"
                );
            return;
        }

        string baseUrl = "http://localhost:5000/api/v1/todolist";
        using var client = new HttpClient();
        string command = args[0].ToLower();

        try
        {
            switch (command)
            {
                case "add":
                    if (args.Length < 3) { Console.WriteLine("Title and description required"); return; }
                    var todoAdd = new { Title = args[1], Description = args[2], Tags = Array.Empty<string>() };
                    var contentAdd = new StringContent(JsonSerializer.Serialize(todoAdd), Encoding.UTF8, "application/json");
                    var responseAdd = await client.PostAsync(baseUrl, contentAdd);
                    Console.WriteLine(responseAdd.IsSuccessStatusCode
                        ? "Todo item added successfully"
                        : $"Error: {responseAdd.StatusCode}");
                    break;

                case "list":
                    var responseList = await client.GetAsync(baseUrl);
                    Console.WriteLine(await responseList.Content.ReadAsStringAsync());
                    break;

                case "get":
                    if (args.Length < 2) { Console.WriteLine("Id required"); return; }
                    var responseGet = await client.GetAsync($"{baseUrl}/{args[1]}");
                    Console.WriteLine(await responseGet.Content.ReadAsStringAsync());
                    break;

                case "delete":
                    if (args.Length < 2) { Console.WriteLine("Id required"); return; }
                    var responseDelete = await client.DeleteAsync($"{baseUrl}/{args[1]}");
                    Console.WriteLine(responseDelete.IsSuccessStatusCode
                        ? "Todo item deleted"
                        : $"Error: {responseDelete.StatusCode}");
                    break;

                case "update":
                    if (args.Length < 4) { Console.WriteLine("Usage: update <id> <title> <description>"); return; }
                    var todoUpdate = new { Id = int.Parse(args[1]), Title = args[2], Description = args[3], Tags = Array.Empty<string>() };
                    var contentUpdate = new StringContent(JsonSerializer.Serialize(todoUpdate), Encoding.UTF8, "application/json");
                    var responseUpdate = await client.PutAsync(baseUrl, contentUpdate);
                    Console.WriteLine(responseUpdate.IsSuccessStatusCode
                        ? "Todo item updated"
                        : $"Error: {responseUpdate.StatusCode}");
                    break;

                default:
                    Console.WriteLine("Unknown command");
                    break;
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Connection error: {ex.Message}");
        }

        Console.ReadLine();
    }
}