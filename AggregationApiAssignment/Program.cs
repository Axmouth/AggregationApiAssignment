using AggregationApiAssignment.Services.ExternalApis;
using AggregationApiAssignment.Services;
using Newtonsoft.Json;
using AggregationApiAssignment.Interfaces;
using NewsAPI.Models;
using Octokit;
using AggregationApiAssignment.Models;
using StackExchange.Redis;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
var config = builder.Configuration.Get<AggregatorConfiguration>() ?? throw new Exception("Failed to get Aggregator Configuration");

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(config.Redis.RedisConnectionString));
builder.Services.AddSingleton(config.GithubApi);
builder.Services.AddSingleton(config.NewsApi);
builder.Services.AddSingleton(config.Redis);
builder.Services.AddSingleton<IExternalApi<List<HackerNewsItem>>, HackerNewsApi>();
builder.Services.AddSingleton<IExternalApi<List<Article>>, NewsApi>();
builder.Services.AddSingleton<IExternalApi<List<Repository>>, GitHubApi>();
builder.Services.AddSingleton<IDataAggregator, DataAggregator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseStatusCodePages();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
