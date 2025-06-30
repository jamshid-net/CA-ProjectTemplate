namespace ProjectTemplate.Shared.PostgresqlCache;
public interface IPostgresCacheService
{
    Task<bool> SetAsync<T>(CacheItem<T> cacheItem, CancellationToken ct = default);
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task<bool> RemoveAsync(string key, CancellationToken ct = default);
}
