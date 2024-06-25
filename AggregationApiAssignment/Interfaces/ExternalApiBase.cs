using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AggregationApiAssignment.Interfaces;

public abstract class ExternalApiBase<T> : IExternalApi<T>
{
    private readonly IConnectionMultiplexer _redis;

    protected ExternalApiBase(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public abstract Task<T> FetchDataAsync(string? query, DateOnly? dateFrom, DateOnly? dateTo);

    public async Task<T> FetchDataAsyncCached(string? query, DateOnly? dateFrom, DateOnly? dateTo)
    {
        string cacheKey = GetCacheKey(query, dateFrom, dateTo);
        var db = _redis.GetDatabase();
        string? cachedData = await db.StringGetAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(cachedData) ?? throw new Exception("Failed to deserialize cached data.");
        }

        T data = await FetchDataAsync(query, dateFrom, dateTo);

        string serializedData = JsonConvert.SerializeObject(data);
        await db.StringSetAsync(cacheKey, serializedData, TimeSpan.FromMinutes(60)); // Cache for 60 minutes

        return data;
    }

    private string GetCacheKey(string? query, DateOnly? dateFrom, DateOnly? dateTo)
    {
        var keyBuilder = new StringBuilder("ExternalApi-");
        keyBuilder.Append(GetType().Name); // Include class name for potential differentiation
        keyBuilder.Append("-");

        if (!string.IsNullOrEmpty(query))
        {
            keyBuilder.Append(query);
        }

        if (dateFrom.HasValue)
        {
            keyBuilder.Append('-').Append(dateFrom.Value.ToString("yyyyMMdd"));
        }

        if (dateTo.HasValue)
        {
            keyBuilder.Append('-').Append(dateTo.Value.ToString("yyyyMMdd"));
        }

        return keyBuilder.ToString();
    }
}
