using System.Threading.Tasks;

namespace DockerDashboard.Infrastructure;

public interface IDockerRegistryProxy
{
    Task<ImageRepositoryDto> GetImageRepositoriesAsync();

    Task<ImageTagDto> GetImageTagsAsync(string image);
}