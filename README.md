## AggregationApiAssignment: Programming News API

This document describes the `ProgrammingNewsController` class within the `AggregationApiAssignment` project. This API endpoint retrieves and aggregates programming news data from various sources.

**Functionality:**

* Fetches data from:
    * Hacker News
    * GitHub API (trending repositories - calculated based on API data)
    * News API (language-specific news)
* Groups fetched data by programming language.
* Filters results based on optional query (language) and date range (from/to).

**API Endpoints:**

* `GET /programming-news`: Retrieves aggregated programming news data.
    * Query Parameters:
        * `query` (string, optional): Filter results to a specific programming language.
        * `dateFrom` (DateOnly, optional): Filter results by date (from). Format: YYYY-MM-DD.
        * `dateTo` (DateOnly, optional): Filter results by date (to). Format: YYYY-MM-DD.

**Response:**

* On successful or partially successful retrieval:
    * Status code: 200 OK
    * Response body: JSON object with the following structure:
        ```json
        [{
          "Language": "string",  // Programming language
          "GitHubRepositories": [ /* Array of GitHub repositories */ ],
          "NewsArticles": [ /* Array of news articles */ ],
          "HackerNewsStories": [ /* Array of Hacker News stories */ ]
          "errors": [] // Empty array if no errors occurred
        },
        ...
        ]
        ```

**Data Model:**

* **AggregatedData:** Represents the aggregated programming news data.
    * `Language` (string): Programming language for which data was aggregated.
    * `GitHubRepositories` (List<Repository>?, optional): Array of GitHub repositories related to the language.
    * `NewsArticles` (List<Article>?, optional): Array of news articles related to the language.
    * `HackerNewsStories` (List<HackerNewsItem>?, optional): Array of Hacker News stories related to the language.
    * `Errors` (List<AggregationError>?, optional): Array containing details about encountered errors during data aggregation.
* **AggregationError:** Represents an error encountered while fetching data from an external API.
    * `ErrorMessage` (string): Description of the error encountered by the specific API.
    * `ErrorSource` (string): Name of the API that failed (e.g., Hacker News).
    * `MissingData` (bool): Boolean flag indicating if the lack of data is due to an API failure.

**Configuration:**

The controller relies on the following configuration settings in `appsettings.json`:

* `NewsApi.NewsApiKey`: API key for accessing News API. (Replace with your actual key)
* `GithubApi.GithubApiToken`: API token for accessing GitHub API. (Replace with your actual token)

**Dependencies:**

* `AggregationApiAssignment.Interfaces.IDataAggregator`: Interface used for data aggregation logic. (Implementation not included here)

**Error Handling:**

The API strives to provide informative error messages in case of failures with any of the external data sources. The response will include an `errors` field with details about encountered issues, including:

* `ErrorMessage`: Description of the error encountered by the specific API.
* `ErrorSource`: Name of the API that failed (e.g., Hacker News).
* `MissingData`: Boolean flag indicating if the lack of data is due to an API failure.

**Example Usage:**

```
GET http://localhost:5000/programming-news?query=python&dateFrom=2024-06-20&dateTo=2024-06-25
```

This request retrieves aggregated programming news data related to Python, filtered for dates between June 20th and 25th, 2024.

**Note:**

* Replace the API keys in `appsettings.json` with your own credentials for the respective

WIP:

Redis caching for API responses (unresolved issue with deserilization from Redis cache to the model object)