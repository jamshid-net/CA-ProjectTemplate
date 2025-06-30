namespace ProjectTemplate.Shared.PostgresqlCache;
public record CacheItem<T>(string Key, T Value, int? ExpirationInSeconds);
