namespace AggregationApiAssignment.Interfaces;

public interface IDataAggregator
{
    /// <summary>
    /// Aggregates data from external APIs.
    /// </summary>
    /// <param name="language">The language to filter the data by.</param>
    /// <param name="dateFrom">The start date to filter the data by.</param>
    /// <param name="dateTo">The end date to filter the data by.</param>
    /// <returns>A Task returning the aggregated data.</returns>
    Task<AggregatedData> AggregateDataAsync(string language, DateOnly? dateFrom, DateOnly? dateTo);

    /// <summary>
    /// Aggregates top headlines from external APIs.
    /// </summary>
    /// <param name="language">The language to filter the data by.</param>
    /// <param name="dateFrom">The start date to filter the data by.</param>
    /// <param name="dateTo">The end date to filter the data by.</param>
    /// <returns>A Task returning a list of aggregated data.</returns>
    /// <remarks>
    /// If language is not provided, the data will be fetched for all languages.
    /// If dateFrom and dateTo are not provided, the data will be fetched for the last day.
    /// </remarks>
    /// <exception cref="Exception">Thrown when an error occurs while fetching the data.</exception>
    /// <exception cref="Exception">Thrown when an error occurs while deserializing the response.</exception>
    Task<List<AggregatedData>> AggregateTopHeadlinesAsync(string? language, DateOnly? dateFrom, DateOnly? dateTo);
}