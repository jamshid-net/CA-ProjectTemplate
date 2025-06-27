using System.Text.Json;

namespace ProjectTemplate.Shared.Models;
public record CacheItem(string Key, JsonElement Value);

