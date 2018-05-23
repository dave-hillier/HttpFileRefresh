using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
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

    public FileRefreshService(IHttpClientFactory clientFactory, IOptions<FileRefreshServiceOptions> options)
    {
      _options = options.Value;
      _client = clientFactory.CreateClient(_options.ClientName);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        var response = await _client.GetAsync(_options.Uri);
        using (var file = File.Create(_options.Path))
        {
          await response.Content.CopyToAsync(file);
        }
        await Task.Delay(_options.Interval, stoppingToken);
      }
    }
  }
}
