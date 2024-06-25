using Xunit;
using Moq;
using AggregationApiAssignment.Interfaces;
using AggregationApiAssignment.Services;
using NewsAPI.Models;
using Octokit;
using AggregationApiAssignment.Services.ExternalApis;
using Newtonsoft.Json;

namespace AggregationApiAssignment.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task AggregateDataAsync_CallsExternalApis_OnSuccess()
        {
            // Arrange
            var mockNewsApi = new Mock<IExternalApi<List<Article>>>();
            var mockGithubApi = new Mock<IExternalApi<List<Repository>>>();
            var mockHackerNewsApi = new Mock<IExternalApi<List<HackerNewsItem>>>();

            string language = "Python";
            DateOnly fromDate = new DateOnly(2024, 06, 20);
            DateOnly toDate = new DateOnly(2024, 06, 24);

            List<Article> newsArticles = new List<Article>();
            List<Repository> githubRepos = new List<Repository>();
            List<HackerNewsItem> hackerNewsStories = new List<HackerNewsItem>();

            mockNewsApi.Setup(api => api.FetchDataAsync(language, fromDate, toDate))
                       .Returns(Task.FromResult(newsArticles));
            mockGithubApi.Setup(api => api.FetchDataAsync(language, fromDate, toDate))
                       .Returns(Task.FromResult(githubRepos));
            mockHackerNewsApi.Setup(api => api.FetchDataAsync(language, fromDate, toDate))
                       .Returns(Task.FromResult(hackerNewsStories));

            var dataAggregator = new DataAggregator(mockNewsApi.Object, mockGithubApi.Object, mockHackerNewsApi.Object);

            // Act
            var aggregatedData = await dataAggregator.AggregateDataAsync(language, fromDate, toDate);

            // Assert
            mockNewsApi.Verify(api => api.FetchDataAsync(language, fromDate, toDate), Times.Once);
            mockGithubApi.Verify(api => api.FetchDataAsync(language, fromDate, toDate), Times.Once);
            mockHackerNewsApi.Verify(api => api.FetchDataAsync(language, fromDate, toDate), Times.Once);

            Assert.Equal(language, aggregatedData.Language);
            Assert.Equal(newsArticles, aggregatedData.NewsArticles);
            Assert.Equal(githubRepos, aggregatedData.GitHubRepositories);
            Assert.Equal(hackerNewsStories, aggregatedData.HackerNewsStories);
            Assert.Null(aggregatedData.Errors);
        }

        [Fact]
        public async Task AggregateDataAsync_HandlesErrors_FromEachApi()
        {
            // Arrange
            var mockNewsApi = new Mock<IExternalApi<List<Article>>>();
            var mockGithubApi = new Mock<IExternalApi<List<Repository>>>();
            var mockHackerNewsApi = new Mock<IExternalApi<List<HackerNewsItem>>>();

            string language = "Python";
            DateOnly fromDate = new DateOnly(2024, 06, 20);
            DateOnly toDate = new DateOnly(2024, 06, 24);

            var newsException = new Exception("News API Error");
            var githubException = new Exception("GitHub API Error");
            var hackerNewsException = new Exception("HackerNews API Error");

            mockNewsApi.Setup(api => api.FetchDataAsync(language, fromDate, toDate))
                       .ThrowsAsync(newsException);
            mockGithubApi.Setup(api => api.FetchDataAsync(language, fromDate, toDate))
                       .ThrowsAsync(githubException);
            mockHackerNewsApi.Setup(api => api.FetchDataAsync(language, fromDate, toDate))
                       .ThrowsAsync(hackerNewsException);

            var dataAggregator = new DataAggregator(mockNewsApi.Object, mockGithubApi.Object, mockHackerNewsApi.Object);

            // Act
            var aggregatedData = await dataAggregator.AggregateDataAsync(language, fromDate, toDate);

            // Assert
            mockNewsApi.Verify(api => api.FetchDataAsync(language, fromDate, toDate), Times.Once);
            mockGithubApi.Verify(api => api.FetchDataAsync(language, fromDate, toDate), Times.Once);
            mockHackerNewsApi.Verify(api => api.FetchDataAsync(language, fromDate, toDate), Times.Once);

            Assert.Equal(language, aggregatedData.Language);
            Assert.Null(aggregatedData.NewsArticles);
            Assert.Null(aggregatedData.GitHubRepositories);
            Assert.Null(aggregatedData.HackerNewsStories);
            Assert.NotNull(aggregatedData.Errors);
            Assert.Equal(3, aggregatedData.Errors.Count);

            // Verify specific errors for each API
            Assert.Contains(aggregatedData.Errors, error => error.ErrorSource == "NewsAPI" && error.ErrorMessage == newsException.Message);
            Assert.Contains(aggregatedData.Errors, error => error.ErrorSource == "GitHub" && error.ErrorMessage == githubException.Message);
            Assert.Contains(aggregatedData.Errors, error => error.ErrorSource == "HackerNews" && error.ErrorMessage == hackerNewsException.Message);
        }

        [Fact]
        public async Task AggregateDataAsync_HandlesErrors_NewsApiFails()
        {
            // Arrange
            var mockNewsApi = new Mock<IExternalApi<List<Article>>>();
            var mockGithubApi = new Mock<IExternalApi<List<Repository>>>();
            var mockHackerNewsApi = new Mock<IExternalApi<List<HackerNewsItem>>>();

            string language = "en";
            DateOnly fromDate = new DateOnly(2024, 06, 20);
            DateOnly toDate = new DateOnly(2024, 06, 24);

            var newsException = new Exception("News API Error");

            // Mock News API to throw an exception
            mockNewsApi.Setup(api => api.FetchDataAsync(language, fromDate, toDate))
                       .ThrowsAsync(newsException);

            // Load sample data from JSON files
            string githubData = File.ReadAllText("../../../GitHubApi-python.json");
            string hackerNewsData = File.ReadAllText("../../../HackerNewsApi-python.json");

            // Deserialize the data (assuming it's a List<T>)
            List<Repository> githubRepos = JsonConvert.DeserializeObject<List<Repository>>(githubData);
            List<HackerNewsItem> hackerNewsStories = JsonConvert.DeserializeObject<List<HackerNewsItem>>(hackerNewsData);

            // Mock successful responses for GitHub and HackerNews
            mockGithubApi.Setup(api => api.FetchDataAsync(language, fromDate, toDate))
                           .Returns(Task.FromResult(githubRepos));
            mockHackerNewsApi.Setup(api => api.FetchDataAsync(language, fromDate, toDate))
                           .Returns(Task.FromResult(hackerNewsStories));

            var dataAggregator = new DataAggregator(mockNewsApi.Object, mockGithubApi.Object, mockHackerNewsApi.Object);

            // Act
            var aggregatedData = await dataAggregator.AggregateDataAsync(language, fromDate, toDate);

            // Assert
            mockNewsApi.Verify(api => api.FetchDataAsync(language, fromDate, toDate), Times.Once);
            mockGithubApi.Verify(api => api.FetchDataAsync(language, fromDate, toDate), Times.Once);
            mockHackerNewsApi.Verify(api => api.FetchDataAsync(language, fromDate, toDate), Times.Once);

            Assert.Equal(language, aggregatedData.Language);
            Assert.Null(aggregatedData.NewsArticles); // News API failed
            Assert.Equal(githubRepos, aggregatedData.GitHubRepositories);
            Assert.Equal(hackerNewsStories, aggregatedData.HackerNewsStories);
            Assert.NotNull(aggregatedData.Errors);
            Assert.Single(aggregatedData.Errors); // Only one error for News API

            // Verify specific error for News API
            Assert.Contains(aggregatedData.Errors, error => error.ErrorSource == "NewsAPI" && error.ErrorMessage == newsException.Message);
        }

    }
}