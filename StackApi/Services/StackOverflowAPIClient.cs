using System.Text.Json;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace StackApi.Services
{
    public class StackOverflowAPIClient
    {
        private readonly HttpClient _client;
        private const string BaseUrl = "https://api.stackexchange.com/2.3";
        private readonly ILogger<StackOverflowAPIClient> _logger;

        public StackOverflowAPIClient(HttpClient client, ILogger<StackOverflowAPIClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<string[]> GetTags(int page = 1, int pageSize = 100)
        {
            var url = $"{BaseUrl}/tags?page={page}&pagesize={pageSize}&order=desc&sort=popular&site=stackoverflow";
            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var tagsResponse = JsonSerializer.Deserialize<StackOverflowTagsResponse>(content);

                if (tagsResponse != null)
                {
                    _logger.LogInformation("Successfully fetched tags from StackOverflow API.");
                    return tagsResponse.Items.Select(tag => tag.Name).ToArray();
                }
            }
            _logger.LogError($"Failed to fetch tags from StackOverflow API. Status code: {response.StatusCode}");
            throw new HttpRequestException($"Failed to fetch tags from StackOverflow API. Status code: {response.StatusCode}");
        }
        public async Task<string[]> RefreshTags()
        {
            // Zwrócenie nowych tagów
            try
            {
                // Użycie istniejącej metody GetTags do pobrania nowych tagów z API StackOverflow
                var tags = await GetTags();

                _logger.LogInformation("Tags refreshed successfully.");

                // Zwrócenie nowych tagów
                return tags;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while refreshing tags.");
                throw;
            }
        }
    }

    public class StackOverflowTagsResponse
    {
        public StackOverflowTag[] Items { get; set; }
    }

    public class StackOverflowTag
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
