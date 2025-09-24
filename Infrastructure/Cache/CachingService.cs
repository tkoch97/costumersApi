using Microsoft.Extensions.Caching.Distributed;

namespace costumersApi.Infrastructure.Cache
{
  public class CachingService : ICachingService
  {
    private readonly IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions _options;

    public CachingService(IDistributedCache cache)
    {
      _cache = cache;
      _options = new DistributedCacheEntryOptions
      {
        // Com o options podemos configurar o tempo de expiração dos dados em cache
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1), // Expiração independente dos dados estarem sendo acessados
        SlidingExpiration = TimeSpan.FromMinutes(30) // Tempo de expiração se os dados não estiverem sendo acessados
      };
    }

    public async Task SetAsync(string key, string value)
    {
      await _cache.SetStringAsync(key, value);
    }

    public async Task<string> GetAsync(string key)
    {
      return await _cache.GetStringAsync(key);
    }
  }
}