using BikeShare.Core.Utils;
using Microsoft.Data.Sqlite;

namespace BikeShare.Core;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // test db connection
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            Console.WriteLine("Database connection successful.");
        }
        
        // Injection here
        builder.Services.AddSingleton<DbConnection>(_ =>
            new DbConnection(builder.Configuration.GetConnectionString("DefaultConnection")!));

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        app.Run();
    }
}