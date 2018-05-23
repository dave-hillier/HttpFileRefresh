using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace HttpFileRefresh
{
  public class AadAuthOptions
  {
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Authority { get; set; }
    public string Resource { get; set; }
  }

  // this class provides authentication in a very similar way to https://github.com/aspnet/Configuration/blob/cfe8c9ee/src/Config.AzureKeyVault/AzureKeyVaultConfigurationExtensions.cs#L68
  // however, I have chosen to use the delegating handler so that the token can be used on multiple requests and new one obtained when expired.
  public class AuthorizationHandler : DelegatingHandler
  {
    private AadAuthOptions _options;
    private string AccessToken = null;
    private DateTimeOffset ExpiresOn;

    public AuthorizationHandler(IOptions<AadAuthOptions> options)
    {
      _options = options.Value;
    }

    private async Task<string> GetAuthToken()
    {
      if (AccessToken != null && (ExpiresOn - TimeSpan.FromSeconds(1)) > DateTime.UtcNow) // TODO: configure timespan before expiry
        return AccessToken;

      var cc = new ClientCredential(_options.ClientId, _options.ClientSecret);
      var ctx = new AuthenticationContext(_options.Authority);
      var tk = await ctx.AcquireTokenAsync(_options.Resource, cc);

      AccessToken = tk.AccessToken;
      ExpiresOn = tk.ExpiresOn;
      return tk.AccessToken;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      var token = await GetAuthToken();
      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
      return await base.SendAsync(request, cancellationToken);
    }
  }

}
