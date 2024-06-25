using AggregationApiAssignment.Services.ExternalApis;
using NewsAPI.Models;
using Octokit;

public class AggregatedData
{
    public required string Language { get; set; }
    public List<Repository>? GitHubRepositories { get; set; }
    public List<Article>? NewsArticles { get; set; }
    public List<HackerNewsItem>? HackerNewsStories { get; set; }
    public List<AggregationError>? Errors { get; set; }
}

public class AggregationError
{
    public required string ErrorMessage { get; set; }
    public required string ErrorSource { get; set; }
    public required bool MissingData { get; set; }
}