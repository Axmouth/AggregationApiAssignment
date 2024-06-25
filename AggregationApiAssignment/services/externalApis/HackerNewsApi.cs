using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AggregationApiAssignment.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AggregationApiAssignment.Services.ExternalApis;

public class HackerNewsApi : ExternalApiBase<List<HackerNewsItem>>
{
    private static readonly HttpClient client = new();
    private const string BaseUrl = "http://hn.algolia.com/api/v1";

    
    public HackerNewsApi(IConnectionMultiplexer redis) : base(redis)
    {
    }

    public override async Task<List<HackerNewsItem>> FetchDataAsync(string? query, DateOnly? dateFrom, DateOnly? dateTo)
    {
        string url = $"{BaseUrl}/search?tags=front_page";
        if (query is not null)
        {
            url = $"{BaseUrl}/search?query={query}&tags=story";
        }
        bool addedNumbericFilters = false;
        if (dateFrom is not null)
        {
            var dateTimeFrom = dateFrom.Value.ToDateTime(TimeOnly.MinValue);
            var timestampFrom = new DateTimeOffset(dateTimeFrom).ToUnixTimeSeconds();
            url += $"&numericFilters=created_at_i>{timestampFrom}";
            addedNumbericFilters = true;
        }
        if (dateTo is not null)
        {
            var dateTimeTo = dateTo.Value.ToDateTime(TimeOnly.MaxValue);
            var timestampTo = new DateTimeOffset(dateTimeTo).ToUnixTimeSeconds();
            if (!addedNumbericFilters)
            {
                url += "&numericFilters=";
            }
            else
            {
                url += ",";
            }
            url += $"created_at_i<{timestampTo}";
        }

        var response = await client.GetStringAsync(url);
        return (JsonConvert.DeserializeObject<HackerNewsResponse>(response) ?? throw new Exception("Failed to deserialize response")).Hits;
    }
}

public class HackerNewsItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public int Points { get; set; }
    public string Type { get; set; }
    public int CreatedAtI { get; set; }
    public string Author { get; set; }
    public int[] Kids { get; set; }
}

public class HackerNewsResponse
{
    public List<HackerNewsItem> Hits { get; set; }
    public int NbHits { get; set; }
    public int Page { get; set; }
    public int NbPages { get; set; }
    public int HitsPerPage { get; set; }
}
