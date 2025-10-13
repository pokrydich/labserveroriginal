namespace Server.IntegrationTests.Database;

using Npgsql;

public class PostgreSqlConnectivityTests
{
    private const string DefaultConnection = "Host=localhost;Port=5432;Database=labs;Username=postgres;Password=123456;Timeout=10";

    [Fact]
    public async Task CanOpenConnection()
    {
        var connectionString = Environment.GetEnvironmentVariable("LABSERVER_DB_CONNECTION") ?? DefaultConnection;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        Assert.Equal(System.Data.ConnectionState.Open, connection.State);

        await using var command = new NpgsqlCommand("SELECT current_database();", connection);
        var result = await command.ExecuteScalarAsync();
        Assert.Equal("labs", result?.ToString());
    }
}
