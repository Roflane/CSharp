using Microsoft.EntityFrameworkCore;
using ToDoListWebApplication.Models;
using ToDoListWebApplication.Services;
using ToDoListWebApplication.Todo;

static class Program {
    static void Main(string[] args) {
        Receiver.Run(args);
    }
}

static class Receiver {
    public static void Run(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("sqlsettings.json", optional: false, reloadOnChange: true);

        builder.Services.AddDbContext<ToDoContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
        
        builder.Services.AddScoped<IToDoListService, ToDoListService>();

        var app = builder.Build();

        app.UseHttpsRedirection();
        
        app.MapGet("api/v1/todolist", async (IToDoListService service) =>
            await service.GetAll());

        app.MapGet("api/v1/todolist/{id:int}", async (int id, IToDoListService service) =>
            await service.GetById(id));

        app.MapPost("api/v1/todolist", async (ToDoItem item, IToDoListService service) =>
        {
            await service.Add(item);
            return Results.Created($"/api/v1/todolist/{item.Id}", item);
        });

        app.MapPut("api/v1/todolist", async (ToDoItem item, IToDoListService service) =>
        {
            await service.Update(item);
            return Results.Ok();
        });

        app.MapDelete("api/v1/todolist/{id:int}", async (int id, IToDoListService service) =>
        {
            await service.Delete(id);
            return Results.Ok();
        });

        app.Run();
    }
} 