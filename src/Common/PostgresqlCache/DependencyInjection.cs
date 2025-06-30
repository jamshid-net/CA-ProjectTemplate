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

        builder.Services.AddHostedService(provider =>
        {
            var dataSource = provider.GetRequiredService<NpgsqlDataSource>();
            InitCacheTable(dataSource);
            return new CacheCleanerService(dataSource);
        });
    }

    private static void InitCacheTable(NpgsqlDataSource dataSource)
    {
        
        using var connection = dataSource.OpenConnection();

        var commandText = """
                              CREATE UNLOGGED TABLE IF NOT EXISTS cache (
                                  id SERIAL PRIMARY KEY,
                                  key TEXT NOT NULL UNIQUE,
                                  value JSONB,
                                  expiration TIMESTAMP WITH TIME ZONE,
                                  created TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                              );
                          
                              CREATE INDEX IF NOT EXISTS idx_cache_key ON cache(key);
                          """;

        using var command = connection.CreateCommand();
        command.CommandText = commandText;
        command.ExecuteNonQuery();

    }
}
