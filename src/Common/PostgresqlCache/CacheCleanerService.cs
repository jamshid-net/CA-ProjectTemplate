using Microsoft.Extensions.Hosting;
using Npgsql;

namespace ProjectTemplate.Shared.PostgresqlCache;
internal sealed class CacheCleanerService(NpgsqlDataSource dataSource) : BackgroundService
{
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var conn = await dataSource.OpenConnectionAsync(stoppingToken);
            var cmd = new NpgsqlCommand("DELETE FROM cache WHERE expiration IS NOT NULL AND expiration <= @now", conn);
            cmd.Parameters.AddWithValue("now", DateTime.UtcNow);
            await cmd.ExecuteNonQueryAsync(stoppingToken);

            Console.WriteLine(DateTime.UtcNow);
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
