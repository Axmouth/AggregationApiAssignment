using AggregationApiAssignment.Interfaces;
using AggregationApiAssignment.Services.ExternalApis;
using NewsAPI.Models;
using Octokit;

namespace AggregationApiAssignment.Services
{
    public class DataAggregator : IDataAggregator
    {
        private readonly IExternalApi<List<Article>> _newsApi;
        private readonly IExternalApi<List<Repository>> _githubApi;
        private readonly IExternalApi<List<HackerNewsItem>> _hackerNewsApi;

        public DataAggregator(IExternalApi<List<Article>> newsApi, IExternalApi<List<Repository>> githubApi, IExternalApi<List<HackerNewsItem>> hackerNewsApi)
        {
            _newsApi = newsApi;
            _githubApi = githubApi;
            _hackerNewsApi = hackerNewsApi;
        }

        public async Task<AggregatedData> AggregateDataAsync(string language, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var aggregatedData = new AggregatedData
            {
                Language = language,
                Errors = new List<AggregationError>()
            };

            var newsTask = FetchDataWithErrorHandlingAsync(
                () => _newsApi.FetchDataAsync(language, dateFrom, dateTo),
                "NewsAPI"
            );

            var githubTask = FetchDataWithErrorHandlingAsync(
                () => _githubApi.FetchDataAsync(language, dateFrom, dateTo),
                "GitHub"
            );

            var hackerNewsTask = FetchDataWithErrorHandlingAsync(
                () => _hackerNewsApi.FetchDataAsync(language, dateFrom, dateTo),
                "HackerNews"
            );

            await Task.WhenAll(newsTask, githubTask, hackerNewsTask);

            aggregatedData.NewsArticles = newsTask.Result.Data;
            if (newsTask.Result.Error != null)
            {
                aggregatedData.Errors.Add(newsTask.Result.Error);
            }

            aggregatedData.GitHubRepositories = githubTask.Result.Data;
            if (githubTask.Result.Error != null)
            {
                aggregatedData.Errors.Add(githubTask.Result.Error);
            }

            aggregatedData.HackerNewsStories = hackerNewsTask.Result.Data;
            if (hackerNewsTask.Result.Error != null)
            {
                aggregatedData.Errors.Add(hackerNewsTask.Result.Error);
            }

            if (aggregatedData.Errors.Count == 0)
            {
                aggregatedData.Errors = null;
            }

            return aggregatedData;
        }

        public async Task<List<AggregatedData>> AggregateTopHeadlinesAsync(string? language, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var languages = Util.Languages;
            if (language != null)
            {
                languages = new List<string> { language };
            }
            var tasks = languages.Select(language => AggregateDataAsync(language, dateFrom, dateTo)).ToList();

            return (await Task.WhenAll(tasks)).ToList();
        }

        private async Task<(T? Data, AggregationError? Error)> FetchDataWithErrorHandlingAsync<T>(Func<Task<T>> fetchDataFunc, string source) where T: class
        {
            try
            {
                var data = await fetchDataFunc();
                return (data, null);
            }
            catch (Exception ex)
            {
                return (null, new AggregationError
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = source,
                    MissingData = true
                });
            }
        }
    }
}
