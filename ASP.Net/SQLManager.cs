using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

internal static class Program {
    static void Main() {
        string tBooks = "Books";
        SQLManager sql = new();
        var columnCount = sql.GetColumnCount(tBooks);

        sql.ExecuteNonQuery($"insert into {tBooks}(Title, Author, YearPublished) values ('GAF', 'Tony', '2025')");
        sql.ExecuteNonQuery($"insert into {tBooks}(Title, Author, YearPublished) values ('Anyways', 'Tony', '2025')");
        sql.ExecuteReader($"select * from {tBooks}", columnCount); 
        Console.WriteLine(sql.GetDataCount(tBooks));
        sql.ExecuteReader($"select Title from {tBooks} where Author = 'Tony'");
         sql.ExecuteScalar($"delete from {tBooks} where Id = 2");
      
        string paramName = "@Id";
        int id = 1002;
        sql.ExecuteReaderSafe($"select * from {tBooks} where Id = {paramName}", paramName, System.Data.SqlDbType.Int, id, columnCount);
    }
}

public class SQLManager {
    private readonly SqlConnection _connection;
    
    public SQLManager() {
        var connectionString = new ConfigurationBuilder()
            .AddJsonFile("sqlsettings.json")
            .Build()
            .GetConnectionString("Default") ?? string.Empty;
        _connection = new SqlConnection(connectionString);
    }

    public void ExecuteReader(string command, int columnCount = 1) {
        var cmd = new SqlCommand(command, _connection);
        _connection.Open();

        using SqlDataReader reader = cmd.ExecuteReader();
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        while (reader.Read()) {
            for (int i = 0; i < columnCount; ++i) {
                Console.WriteLine(reader[i]);
            }
            Console.WriteLine();
        }

        _connection.Close();
        Console.ResetColor();
    }

    public void ExecuteReaderSafe(string command, string paramName, System.Data.SqlDbType dataType, object value, int columnCount = 1) {
        var cmd = new SqlCommand(command, _connection);
        cmd.Parameters.Add(new SqlParameter(paramName, dataType));
        cmd.Parameters[paramName].Value = value;
        _connection.Open();

        using SqlDataReader reader = cmd.ExecuteReader();
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        while (reader.Read()) {
            for (int i = 0; i < columnCount; ++i) {
                Console.WriteLine(reader[i]);
            }
            Console.WriteLine();
        }

        _connection.Close();
        Console.ResetColor();
    }

    public void ExecuteNonQuery(string command) {
        var cmd = new SqlCommand(command, _connection);
        _connection.Open();
        var res = cmd.ExecuteNonQuery();
        _connection.Close();
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"numbers of rows affected: {res}");
        Console.ResetColor();
    }

    public object? ExecuteScalar(string command) {
        var cmd = new SqlCommand(command, _connection);
        _connection.Open();

        var res = cmd.ExecuteScalar();
        _connection.Close();
        return res;
    }
    
    public int GetColumnCount(string tableName) {
        object? res = ExecuteScalar($"""
                                     select COUNT(*)
                                     from INFORMATION_SCHEMA.COLUMNS
                                     where TABLE_NAME = '{tableName}';
                                     """);
        return (int)res!;
    }
    
    public int GetDataCount(string tableName) {
        object? res = ExecuteScalar($"""
                                     select COUNT(*)
                                     from {tableName}
                                     """);
        return (int)res!;
    }
}