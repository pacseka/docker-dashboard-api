using System.Threading.Tasks;
using DockerDashboard.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DockerDashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistryController : ControllerBase
    {
        private readonly IDockerRegistryProxy _dockerRegistryProxy;

        public RegistryController(IDockerRegistryProxy dockerRegistryProxy)
        {
            _dockerRegistryProxy = dockerRegistryProxy;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ImageRepositoryDto>> GetImages()
        {
            return Ok(await _dockerRegistryProxy.GetImageRepositoriesASync().ConfigureAwait(false));
        }

        [HttpGet("tags")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ImageTagDto>> GetTags([FromQuery] string imageName)
        {
            return Ok(await _dockerRegistryProxy.GetImageTagsAsync(imageName).ConfigureAwait(false));
        }
    }
}
