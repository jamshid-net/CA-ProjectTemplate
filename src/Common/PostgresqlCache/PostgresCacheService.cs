using System.Text.Json;
using Npgsql;
using NpgsqlTypes;

namespace ProjectTemplate.Shared.PostgresqlCache;
internal class PostgresCacheService(NpgsqlDataSource dataSource) : IPostgresCacheService
{
    public async Task<bool> SetAsync<T>(CacheItem<T> cacheItem, CancellationToken ct = default)
    {

        await using var conn = await dataSource.OpenConnectionAsync(ct);
        var commandText = @"
            INSERT INTO cache (key, value, expiration)
            VALUES (@key, @value, @expiration)
            ON CONFLICT (key) DO UPDATE
            SET value = EXCLUDED.value,
                expiration = EXCLUDED.expiration;";

        await using var cmd = new NpgsqlCommand(commandText, conn);
        cmd.Parameters.AddWithValue("key", cacheItem.Key);
        cmd.Parameters.AddWithValue("value", NpgsqlDbType.Jsonb, JsonSerializer.Serialize(cacheItem.Value));
        cmd.Parameters.AddWithValue("expiration", cacheItem.ExpirationInSeconds.HasValue ? DateTime.UtcNow.AddSeconds(cacheItem.ExpirationInSeconds.Value) : DBNull.Value);

        return await cmd.ExecuteNonQueryAsync(ct) > 0;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct);

        var commandText = @"
            SELECT value FROM cache
            WHERE key = @key
            AND (expiration IS NULL OR expiration > @now)";

        await using var cmd = new NpgsqlCommand(commandText, conn);
        cmd.Parameters.AddWithValue("key", key);
        cmd.Parameters.AddWithValue("now", DateTime.UtcNow);

        var result = await cmd.ExecuteScalarAsync(ct);

        return result is null ? default : JsonSerializer.Deserialize<T>(result.ToString()!);
    }

    public async Task<bool> RemoveAsync(string key, CancellationToken ct = default)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct);

        var commandText = "DELETE FROM cache WHERE key = @key";

        await using var cmd = new NpgsqlCommand(commandText, conn);
        cmd.Parameters.AddWithValue("key", key);

        return await cmd.ExecuteNonQueryAsync(ct) > 0;
    }
}
