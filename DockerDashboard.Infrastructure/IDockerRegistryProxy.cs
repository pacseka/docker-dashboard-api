using System.Threading.Tasks;

namespace DockerDashboard.Infrastructure
{
    public interface IDockerRegistryProxy
    {
        Task<ImageRepositoryDto> GetImageRepositoriesASync();
        Task<ImageTagDto> GetImageTagsAsync(string image);
    }
}