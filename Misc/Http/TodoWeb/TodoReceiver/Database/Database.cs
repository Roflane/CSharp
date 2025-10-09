using System;
using System.Data;
using System.IO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public static class Database {
    private static string GetConnectionString() {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("sqlsettings.json", optional: false)
            .Build();

        return configuration.GetConnectionString("Default");
    }

    public static void InsertTodo(string title, string description) {
        var connectionString = GetConnectionString();

        using var conn = new SqlConnection(connectionString);
        conn.Open();

        using var cmd = new SqlCommand(
            "INSERT INTO TodoItems (Title, Description) VALUES (@title, @description)", conn);
        cmd.Parameters.AddWithValue("@title", title);
        cmd.Parameters.AddWithValue("@description", description);

        cmd.ExecuteNonQuery();

        Console.WriteLine("Todo added successfully!");
    }

    public static void PrintAllTodos() {
        var connectionString = GetConnectionString();

        using var conn = new SqlConnection(connectionString);
        conn.Open();

        using var cmd = new SqlCommand("SELECT Id, Title, Description FROM TodoItems", conn);
        using var reader = cmd.ExecuteReader();

        Console.WriteLine("\n📋 Todo List:");
        while (reader.Read()) {
            Console.WriteLine($"{reader["Id"]}: {reader["Title"]} — {reader["Description"]}");
        }
    }
}