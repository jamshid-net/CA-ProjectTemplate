using Microsoft.AspNetCore.Http.HttpResults;
using ProjectTemplate.Shared.PostgresqlCache;

namespace ProjectTemplate.Web.Endpoints;

public class TestPostgresqlCache : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetValue)
            .MapPost(SetValue)
            .MapDelete(RemoveValue);

    }

    public async Task<Results<Ok<string>, NotFound>> GetValue(IPostgresCacheService service, string key, CancellationToken ct)
    {
        var value = await service.GetAsync<string>(key, ct);

        return value is not null ? TypedResults.Ok(value) : TypedResults.NotFound();
    }

    public async Task<Results<Created, BadRequest>> SetValue(IPostgresCacheService service, CacheItem<string> cacheItem, CancellationToken ct)
    {
        var isCreated = await service.SetAsync(cacheItem, ct);

        return isCreated ? TypedResults.Created() : TypedResults.BadRequest();
    }

    public async Task<Results<NoContent, NotFound>> RemoveValue(IPostgresCacheService service, string key, CancellationToken ct)
    {
        var isRemoved = await service.RemoveAsync(key, ct);
        return  isRemoved ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
