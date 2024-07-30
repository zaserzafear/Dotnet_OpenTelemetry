using Microsoft.Extensions.Options;
using MyApp1.Models;
using System.Diagnostics;

namespace MyApp1.Services
{
    public class MyApp2Client
    {
        private readonly ILogger<MyApp2Client> _logger;
        private readonly HttpClient _httpClient;
        private readonly MyApp2Settings _settings;
        private readonly ActivitySource _activitySource;

        public MyApp2Client(ILogger<MyApp2Client> logger, HttpClient httpClient, IOptions<MyApp2Settings> settings, ActivitySource activitySource)
        {
            _logger = logger;
            _httpClient = httpClient;
            _settings = settings.Value;
            _activitySource = activitySource;

            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.Timeout);
        }

        public async Task<string> CallServiceAsync(HttpMethod method, string endpoint, HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method, $"{_settings.BaseUrl}{endpoint}")
            {
                Content = content
            };

            using var activity = _activitySource.StartActivity("CallServiceAsync", ActivityKind.Client);
            if (activity != null)
            {
                activity.SetTag("http.method", method.Method);
                activity.SetTag("http.url", request.RequestUri);
                activity.SetTag("http.endpoint", endpoint);
            }

            try
            {
                _logger.LogInformation("Sending request to {Endpoint}", endpoint);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully received response from {Endpoint}", endpoint);
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    _logger.LogError("Request to {Endpoint} failed with status code {StatusCode}", endpoint, response.StatusCode);
                    activity?.SetStatus(ActivityStatusCode.Error, $"Request failed with status code {response.StatusCode}");
                    response.EnsureSuccessStatusCode();
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while calling {Endpoint}", endpoint);
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }
            finally
            {
                if (activity != null)
                {
                    activity.SetTag("http.endpoint", endpoint);
                }
            }
        }
    }
}
