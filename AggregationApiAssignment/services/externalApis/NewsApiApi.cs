using System;
using System.Threading.Tasks;
using AggregationApiAssignment.Interfaces;
using AggregationApiAssignment.Models;
using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
using StackExchange.Redis;

namespace AggregationApiAssignment.Services.ExternalApis;

public class NewsApi : ExternalApiBase<List<Article>>
{
    private readonly NewsApiClient _client;

    public NewsApi(IConnectionMultiplexer redis, NewsApiConfig config) : base(redis)
    {
        _client = new NewsApiClient(config.NewsApiKey);
    }

    public override async Task<List<Article>> FetchDataAsync(string? query, DateOnly? dateFrom, DateOnly? dateTo)
    {
        if (query is null)
        {
            query = "programming";
        }
        var request = new EverythingRequest
        {
            Q = query,
            From = dateFrom?.ToDateTime(TimeOnly.MinValue),
            To = dateTo?.ToDateTime(TimeOnly.MaxValue),
            SortBy = SortBys.PublishedAt,
            Language = Languages.EN,
        };

        var response = await _client.GetEverythingAsync(request);
        if (response.Status != Statuses.Ok)
        {
            throw new Exception($"Error fetching news: {response.Error.Message}");
        }

        return response.Articles;
    }
}
