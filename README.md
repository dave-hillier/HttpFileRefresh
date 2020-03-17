# HttpFileRefresh
Fetches a resource periodically and writes to a file.

[![Travis](https://travis-ci.org/dave-hillier/HttpFileRefresh.svg?branch=master)](https://travis-ci.org/dave-hillier/HttpFileRefresh)

Simple example:

```
  services.AddHttpClient("client");
  
  services.Configure<FileRefreshServiceOptions>(options => {
    options.ClientName = "client";
    options.Interval = TimeSpan.FromMinutes(60);
    options.Uri = new Uri("https://httpbin.org/get");
    options.Path = "whatever.json";
  });
  services.AddSingleton<IHostedService, FileRefreshService>();
```
