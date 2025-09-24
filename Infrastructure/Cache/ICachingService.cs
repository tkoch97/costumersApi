namespace costumersApi.Infrastructure.Cache
{
  public interface ICachingService
  {
    Task SetAsync(string key, string value);
    Task<String> GetAsync(string key);
  }
}