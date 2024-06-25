using System.Text;
using AggregationApiAssignment.Models;
using StackExchange.Redis;

namespace AggregationApiAssignment.Interfaces;

public interface IExternalApi<T>
{
    /// <summary>
    /// Fetches data from an external API.
    /// </summary>
    /// <param name="query">The query to filter the data by.</param>
    /// <param name="dateFrom">The start date to filter the data by.</param>
    /// <param name="dateTo">The end date to filter the data by.</param>
    /// <returns>A Task returning the fetched data.</returns>
    /// <remarks>
    /// The query parameter is optional and can be used to filter the data by a specific query.
    /// The dateFrom and dateTo parameters are optional and can be used to filter the data by a specific date range.
    /// </remarks>
    /// <exception cref="Exception">Thrown when the response cannot be deserialized.</exception>
    /// <exception cref="Exception">Thrown when an error occurs while fetching the data.</exception>
    /// <exception cref="Exception">Thrown when an error occurs while deserializing the response.</exception>
    /// <exception cref="Exception">Thrown when an error occurs while fetching the data.</exception>
    Task<T> FetchDataAsync(string? query, DateOnly? dateFrom, DateOnly? dateTo);

    Task<T> FetchDataAsyncCached(string? query, DateOnly? dateFrom, DateOnly? dateTo) {
        // Implement caching logic here

        return FetchDataAsync(query, dateFrom, dateTo);
    }

    private string GetCacheKey(string? query, DateOnly? dateFrom, DateOnly? dateTo)
    {
        var keyBuilder = new StringBuilder("ExternalApi-");
        keyBuilder.Append(GetType().Name); // Include class name for potential differentiation
        keyBuilder.Append("-");
        // Add query, dateFrom, and dateTo values in a serialized format (e.g., JSON)
        return keyBuilder.ToString();
    }
}

