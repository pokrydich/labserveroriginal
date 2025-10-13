namespace Server.IntegrationTests.Database;

using Npgsql;

using Npgsql;
using System.Data;

public class PostgreSqlConnectivityTests
{
    private const string DefaultConnection =
        "Host=localhost;Port=5432;Database=labs;Username=postgres;Password=123456;Timeout=10";

    private static string GetConnectionString() =>
        Environment.GetEnvironmentVariable("LABSERVER_DB_CONNECTION") ?? DefaultConnection;

    [Fact]
    public async Task CanOpenConnection()
    {
        var connectionString = GetConnectionString();

        await using var connection = new NpgsqlConnection(connectionString);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        await connection.OpenAsync(cts.Token);

        Assert.Equal(ConnectionState.Open, connection.State);

        await using var command = new NpgsqlCommand("SELECT current_database();", connection);
        var result = await command.ExecuteScalarAsync(cts.Token);

        Assert.Equal("labs", result?.ToString());

        await connection.CloseAsync();
        Assert.Equal(ConnectionState.Closed, connection.State);
    }

    [Fact]
    public async Task Database_ShouldContainPublicTables()
    {
        var connectionString = GetConnectionString();

        await using var connection = new NpgsqlConnection(connectionString);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        await connection.OpenAsync(cts.Token);

        await using var cmd = new NpgsqlCommand(
            "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public';",
            connection);

        var tableCount = (long)(await cmd.ExecuteScalarAsync(cts.Token) ?? 0);
        Assert.True(tableCount > 0, "Expected at least one table in the 'public' schema.");

        await connection.CloseAsync();
    }
}
