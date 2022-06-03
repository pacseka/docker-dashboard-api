using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DockerDashboard.Infrastructure;

public class DockerRegistryProxy : IDockerRegistryProxy
{
    private readonly HttpClient _httpClient;

    public DockerRegistryProxy(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ImageRepositoryDto> GetImageRepositoriesAsync()
    {
        var result = await _httpClient.GetAsync("_catalog").ConfigureAwait(false);

        var json = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<ImageRepositoryDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public async Task<ImageTagDto> GetImageTagsAsync(string image)
    {
        var result = await _httpClient.GetAsync($"{image}/tags/list").ConfigureAwait(false);

        var json = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<ImageTagDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}