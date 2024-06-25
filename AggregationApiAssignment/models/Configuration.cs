namespace AggregationApiAssignment.Models;

public class AggregatorConfiguration {
    public RedditApiConfig RedditApi { get; set; }
    public required NewsApiConfig NewsApi { get; set; }
    public required GithubApiConfig GithubApi { get; set; }
    public required RedisConfig Redis { get; set; }
}

public class RedditApiConfig {
    public required string BotRefreshToken { get; set; }
    public required string ClientId { get; set; }
}

public class NewsApiConfig {
    public required string NewsApiKey { get; set; }
}

public class GithubApiConfig {
    public required string GithubApiToken { get; set; }
}

public class RedisConfig {
    public required string RedisConnectionString { get; set; }
}