using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Domain.Entities.Auth;
using ProjectTemplate.Infrastructure.Data;

namespace BenchmarkTesting;
[MemoryDiagnoser]

public class Benchmarks
{
   
    private  IServiceScope _scope = null!;
    private  ApplicationDbContext dbContext = null!;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql("Host=127.0.0.1;Port=5432;Database=ProjectTemplateDb;Username=postgres;Password=Jam568$;").UseSnakeCaseNamingConvention();
        });

        services.AddScoped<ApplicationDbContext>();

        var provider = services.BuildServiceProvider();

        _scope = provider.CreateScope();
        dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
    [Benchmark]
    public List<User> GetUsersWithNoTrackingWithIdentityResolution()
    {
       
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;

        return dbContext.Users.ToList();
    }

    [Benchmark]
    public List<User> GetUsersWithNoTracking()
    {
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        return dbContext.Users.ToList();
    }

    
    [Benchmark]
    public List<User> GetUsersWithTrackAll()
    {
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

        return dbContext.Users.ToList();
    }



}
[MemoryDiagnoser]
[ThreadingDiagnoser]
[DisassemblyDiagnoser]
[HardwareCounters(HardwareCounter.BranchMispredictions, HardwareCounter.CacheMisses)]
public class StringBenchmarks
{
    private readonly int[] _numbers = Enumerable.Range(1, 1000).ToArray();

    [Benchmark]
    public int ForLoopSum()
    {
        int sum = 0;
        for (int i = 0; i < _numbers.Length; i++)
        {
            sum += _numbers[i];
        }
        return sum;
    }

    [Benchmark]
    public int LinqSum()
    {
        return _numbers.Sum();
    }

    [Benchmark]
    public int ForEachLoop()
    {
        int sum = 0;
        foreach (int t in _numbers)
        {
            sum += t;
        }
        return sum;
    }
}


[MemoryDiagnoser]
public class TestCache
{
    //private PostgresCacheService _cache = null!;
    //private IServiceScope _scope = null!;
    //private IDistributedCache _redisCache = null!;

    //[GlobalSetup]
    //public void Setup()
    //{
    //    var services = new ServiceCollection();
    //    services.AddStackExchangeRedisCache(options =>
    //    {
    //        options.Configuration = "localhost";
    //        options.InstanceName = "benchmark";
    //    });
    //    _scope = services.BuildServiceProvider().CreateScope();
    //    _redisCache = _scope.ServiceProvider.GetRequiredService<IDistributedCache>();
    //    var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder("Server=127.0.0.1;Port=5432;Database=ProjectTemplateDb;Username=postgres;Password=Jam568$;");
    //    var dataSource = dataSourceBuilder.Build();
    //    _cache = new PostgresCacheService(dataSource);
    ///}

    #region SET_TEST
    //@@ MEAN: 344.6 ms, Error: 6.845 ms, StdDev: 16.790 ms, Allocated: 2890.43 KB
    //[Benchmark]
    //public async Task SetPostgresqlCache()
    //{
    //    for (int i = 0; i < 1000; i++)
    //    {
    //        var cacheItem = new CacheItem<string>($"testKey{i}", $"TestValue{i}", null);

    //        await _cache.SetAsync(cacheItem);
    //    }
    //}

    //@@ MEAN: 44.03 ms, Error: 0.404 ms, StdDev: 0.358 ms, Allocated: 983.76 KB
    //[Benchmark]
    //public async Task SetRedisCache()
    //{
    //    for (int i = 0; i < 1000; i++)
    //    {
    //        await _redisCache.SetStringAsync($"testKey{i}", $"TestValue{i}");
    //    }
    //}
    #endregion

    #region GET_TEST
    //338.77 ms | 8.653 ms | 25.104 ms | 327.09 ms |   2.49 MB
    //[Benchmark]
    //public async Task<List<string?>> GetPostgresqlCache()
    //{
    //    List<string?> strings = [];
    //    for (int i = 0; i < 1000; i++)
    //    {
    //        var res = await _cache.GetAsync<string>($"testKey{i}");
    //        strings.Add(res);
    //    }
    //    return strings;
    //}


    // 43.37 ms | 0.864 ms |  0.887 ms |  43.03 ms |   1.05 MB 
    //[Benchmark]
    //public async Task<List<string?>> GetRedisCache()
    //{
    //    List<string?> strings = [];
    //    for (int i = 0; i < 1000; i++)
    //    {
    //        var res = await _redisCache.GetStringAsync($"testKey{i}");
    //        strings.Add(res);
    //    }
    //    return strings;
    //}

    #endregion
}
