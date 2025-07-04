using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Domain.Entities.Auth;
using ProjectTemplate.Infrastructure.Data;
using ProjectTemplate.Shared.PostgresqlCache;

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
    public PostgresCacheService cache = null!;
    public IServiceScope _scope = null!;
    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();



        var provider = services.BuildServiceProvider();

        _scope = provider.CreateScope();
    }
}
