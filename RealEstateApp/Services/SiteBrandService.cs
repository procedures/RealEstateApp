using Microsoft.Extensions.Caching.Memory;
using RealEstateApp.Data.Repositories;

namespace RealEstateApp.Services;

public interface ISiteBrandService
{
    Task<string> GetCompanyNameAsync();
    Task<string> GetSloganAsync();
    void Invalidate();
}

public class SiteBrandService : ISiteBrandService
{
    private const string KeyName = "brand:company_name";
    private const string KeySlogan = "brand:company_slogan";
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);

    private readonly IMemoryCache _cache;
    private readonly IServiceScopeFactory _scopeFactory;

    public SiteBrandService(IMemoryCache cache, IServiceScopeFactory scopeFactory)
    {
        _cache = cache;
        _scopeFactory = scopeFactory;
    }

    public Task<string> GetCompanyNameAsync() => GetAsync(KeyName, "company_name", "ҮХХ Портал");
    public Task<string> GetSloganAsync() => GetAsync(KeySlogan, "company_slogan", "");

    private async Task<string> GetAsync(string cacheKey, string settingKey, string fallback)
    {
        if (_cache.TryGetValue<string>(cacheKey, out var cached) && cached is not null)
            return cached;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ISiteRepository>();
            var bag = await repo.GetSettingsAsync();

            var value = bag[settingKey];
            if (string.IsNullOrWhiteSpace(value)) value = fallback;

            _cache.Set(cacheKey, value, Ttl);
            return value;
        }
        catch
        {
            return fallback;   // бааз унасан ч толгой хоосон харагдахгүй
        }
    }

    public void Invalidate()
    {
        _cache.Remove(KeyName);
        _cache.Remove(KeySlogan);
    }
}