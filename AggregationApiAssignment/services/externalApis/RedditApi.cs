using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AggregationApiAssignment.Interfaces;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs.Search;

namespace AggregationApiAssignment.Services.ExternalApis
{
    public class RedditApi : IExternalApi<List<Post>>
    {
        private readonly RedditClient _client;
        private readonly List<string> _subreddits;

        public RedditApi(string clientId, string refreshToken)
        {
            _client = new RedditClient(clientId, refreshToken);
            _subreddits = new List<string>
            {
                "programming",
                "technology",
                "learnprogramming",
                "csharp",
                "javascript",
                "python",
                "java",
                "cpp",
                "golang",
                "rust"
            };
        }

        public async Task<List<Post>> FetchDataAsync(string? query, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var posts = new List<Post>();

            foreach (var subreddit in _subreddits)
            {
                var searchPosts = _client.Subreddit(subreddit).Search(new SearchGetSearchInput(query));
                posts.AddRange(searchPosts);
            }

            if (dateFrom is not null || dateTo is not null)
            {
                var from = dateFrom?.ToDateTime(TimeOnly.MinValue);
                var to = dateTo?.ToDateTime(TimeOnly.MaxValue);

                posts = posts.FindAll(post =>
                {
                    var postDate = new DateTimeOffset(post.Created);
                    return (!from.HasValue || postDate >= from) && (!to.HasValue || postDate <= to);
                });
            }

            return await Task.FromResult(posts);
        }
    }
}
