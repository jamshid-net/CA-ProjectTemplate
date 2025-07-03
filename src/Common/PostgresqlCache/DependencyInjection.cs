using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace ProjectTemplate.Shared.PostgresqlCache;
public static class DependencyInjection
{
    public static void AddPostgresqlCache(this IHostApplicationBuilder builder, string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
        }
        builder.Services.AddNpgsqlDataSource(connectionString);

        builder.Services.AddSingleton<IPostgresCacheService, PostgresCacheService>();

        builder.Services.AddHostedService<CacheCleanerService>();
    }

    public static async Task UsePostgresqlCacheAsync(this WebApplication app)
    {
        var dataSource = app.Services.GetRequiredService<NpgsqlDataSource>();

        await using var connection = await dataSource.OpenConnectionAsync();

        var commandText = """
                              CREATE UNLOGGED TABLE IF NOT EXISTS cache (
                                  key TEXT PRIMARY KEY,
                                  value JSONB,
                                  expiration TIMESTAMP WITH TIME ZONE,
                                  created TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                              );
                          
                              CREATE INDEX IF NOT EXISTS idx_cache_expiration ON cache(expiration);
                          """;

        await using var command = connection.CreateCommand();
        command.CommandText = commandText;
        await command.ExecuteNonQueryAsync();

    }
}
