using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HttpFileRefresh
{
  public class FileRefreshServiceOptions
  {
    public Uri Uri { get; set; }
    public string Path { get; set; }
    public TimeSpan Interval { get; set; }
    public string ClientName { get; set; }
  }

  public class FileRefreshService : BackgroundService
  {
    HttpClient _client;
    FileRefreshServiceOptions _options;

    public FileRefreshService(ILogger<FileRefreshService> logger, IHttpClientFactory clientFactory, IOptions<FileRefreshServiceOptions> options)
    {
      _options = options.Value;
      _client = clientFactory.CreateClient(_options.ClientName);
      Logger = logger;
    }

    public ILogger<FileRefreshService> Logger { get; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      Logger.LogInformation($"Starting...");
      while (!stoppingToken.IsCancellationRequested)
      {
        Logger.LogInformation($"Fetching {_options.Uri}...");
        var response = await _client.GetAsync(_options.Uri);
        using (var file = File.Create(_options.Path))
        {
          Logger.LogDebug($"Writing to file {file}...");
          await response.Content.CopyToAsync(file);
        }
        Logger.LogDebug($"Delaying task for {_options.Interval}");
          
        await Task.Delay(_options.Interval, stoppingToken);
      }
      Logger.LogInformation($"Cancelled...");
    }
  }
}
