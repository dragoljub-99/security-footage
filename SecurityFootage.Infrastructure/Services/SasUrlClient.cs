using System.Net.Http.Json;

namespace SecurityFootage.Infrastructure.Services
{
    public class SasUrlClient
    {
        private readonly HttpClient _httpClient;

        public SasUrlClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetSasUrlAsync(string blobName, CancellationToken ct)
        {
            DownloadUrlResponse? downloadUrl = await _httpClient.GetFromJsonAsync<DownloadUrlResponse>(
                                                         Uri.EscapeDataString(blobName), ct);
            
            return downloadUrl?.DownloadUrl ?? throw new InvalidOperationException("Url is null");
        }
    }

    public sealed record DownloadUrlResponse(string DownloadUrl);
}