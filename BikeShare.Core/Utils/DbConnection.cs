using Dapper;
using Microsoft.Data.Sqlite;

namespace BikeShare.Core.Utils;

public class DbConnection
{
    private readonly string _connectionString;
    
    public DbConnection(string connectionString)
    {
        _connectionString = connectionString;
        SimpleCRUD.SetDialect(SimpleCRUD.Dialect.SQLite);
    }
    
    public SqliteConnection GetConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }
}