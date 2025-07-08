using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Distributed;
using ProjectTemplate.Application.Common.Interfaces;
using StackExchange.Redis;

namespace ProjectTemplate.Web.Endpoints;

public class TestRedisCache : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapGet(GetValue);
        group.MapPost(SetValue);
        group.MapDelete(RemoveValue);
        group.MapGet(GetAllTest);
    }

    public async Task<Results<Ok<string>, NotFound>> GetValue(IDistributedCache cache, string key, CancellationToken ct)
    {
        var value = await cache.GetStringAsync(key, ct);

        return value is not null ? TypedResults.Ok(value) : TypedResults.NotFound();
    }

    public async Task<Created> SetValue(IDistributedCache cache, string key, JsonValue value, CancellationToken ct)
    {

        var valueToString = value?.ToJsonString() ?? string.Empty;
        await cache.SetStringAsync(key, valueToString, ct);
        return TypedResults.Created();

    }

    public async Task<NoContent> RemoveValue(IDistributedCache cache, string key, CancellationToken ct)
    {
        await cache.RemoveAsync(key, ct);
        return TypedResults.NoContent();
    }


    public async Task<Ok<List<string>>> GetAllTest(IRedisServer redisServer, IDistributedCache cache, CancellationToken ct)
    {
       
        var server = redisServer.Server;
        var keys = server.KeysAsync();
        var keyList = new List<string>();

       
        await foreach (var key in keys)
        {
            
        }



        return TypedResults.Ok(keyList);

    }

}
