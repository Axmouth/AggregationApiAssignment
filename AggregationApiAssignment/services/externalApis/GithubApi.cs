using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AggregationApiAssignment.Interfaces;
using AggregationApiAssignment.Models;
using Octokit;
using StackExchange.Redis;

namespace AggregationApiAssignment.Services.ExternalApis
{
    public class GitHubApi : ExternalApiBase<List<Repository>>
    {
        private readonly GitHubClient _client;

        public GitHubApi(IConnectionMultiplexer redis, GithubApiConfig config) : base(redis)
        {
            _client = new GitHubClient(new ProductHeaderValue("AggregationApiAssignment"))
            {
                Credentials = new Credentials(config.GithubApiToken)
            };
        }

        public override async Task<List<Repository>> FetchDataAsync(string? query, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var request = new SearchRepositoriesRequest(query)
            {
                SortField = RepoSearchSort.Updated,
                Order = SortDirection.Descending
            };

            if (dateFrom is not null && dateTo is not null)
            {
                var fromDateTimeOffset = new DateTimeOffset(dateFrom.Value.ToDateTime(TimeOnly.MinValue));
                var toDateTimeOffset = new DateTimeOffset(dateTo.Value.ToDateTime(TimeOnly.MaxValue));
                request.Created = new DateRange(fromDateTimeOffset, toDateTimeOffset);
            }
            else if (dateFrom is not null)
            {
                var fromDateTimeOffset = new DateTimeOffset(dateFrom.Value.ToDateTime(TimeOnly.MinValue));
                request.Created = new DateRange(fromDateTimeOffset, SearchQualifierOperator.GreaterThan);
            }
            else if (dateTo is not null)
            {
                var toDateTimeOffset = new DateTimeOffset(dateTo.Value.ToDateTime(TimeOnly.MaxValue));
                request.Created = new DateRange(toDateTimeOffset, SearchQualifierOperator.LessThan);
            }

            var result = await _client.Search.SearchRepo(request);
            return [.. result.Items];
        }

        public async Task<List<Repository>> FetchTrendingReposAsync(string language)
        {
            var request = new SearchRepositoriesRequest
            {
                SortField = RepoSearchSort.Stars,
                Order = SortDirection.Descending,
                Language = Util.LanguageFromString(language),
                Created = new DateRange(DateTimeOffset.Now.AddDays(-7), SearchQualifierOperator.GreaterThan)
            };

            var result = await _client.Search.SearchRepo(request);
            return [.. result.Items];
        }
    }
}
