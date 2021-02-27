using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DockerDashboard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly ILogger<ServicesController> _logger;
        private readonly Uri _swarmUrl;

        public ServicesController(ILogger<ServicesController> logger)
        {
            _logger = logger;
            _swarmUrl = new Uri(Environment.GetEnvironmentVariable("SWARM_URL") ?? string.Empty);
        }

        [HttpGet]
        public async Task<IActionResult> GetServices([FromQuery] ServiceFilter filter)
        {
            List<SwarmService> services;

            try
            {
                using var dockerClientConfiguration =
                    new DockerClientConfiguration(_swarmUrl, defaultTimeout: TimeSpan.FromSeconds(10));
                using var dockerClient = dockerClientConfiguration.CreateClient();
                services = (await dockerClient.Swarm.ListServicesAsync().ConfigureAwait(false)).ToList();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Hiba a swarm megszólításakor!");
                throw;
            }

            var serviceWithEndpoints = services.Where(x => x.Endpoint.Ports != null).ToList();

            var result = new List<SwarmServiceDto>();

            foreach (var service in serviceWithEndpoints)
            {
                var swagger = service
                    .Spec
                    .TaskTemplate?
                    .ContainerSpec?
                    .Env?
                    .FirstOrDefault(x => x.StartsWith("SWAGGER_SERVER", StringComparison.InvariantCultureIgnoreCase));

                if (string.IsNullOrEmpty(swagger) && filter?.SwaggerExists == true)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(swagger) && filter?.SwaggerExists == false)
                {
                    result.Add(new SwarmServiceDto
                    {
                        Name = service.Spec.Name,
                        Ports = service.Endpoint.Ports,
                    });

                    continue;
                }

                result.Add(new SwarmServiceDto
                {
                    Name = service.Spec.Name,
                    Ports = service.Endpoint.Ports,
                    SwaggerUrl = CreateSwaggerUri(swagger ?? string.Empty)
                });
            }

            return Ok(result);
        }

#nullable enable
        private static Uri? CreateSwaggerUri(string url)
#nullable disable
        {
            var swaggerUrl = url
                .Replace("SWAGGER_SERVER:", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                .Replace("SWAGGER_SERVER=", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                .Trim()
                .TrimEnd('/');

            swaggerUrl = $"{swaggerUrl}/swagger";

            if (!swaggerUrl.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
            {
                swaggerUrl = $"http://{swaggerUrl}";
            }

            Uri.TryCreate(swaggerUrl, UriKind.Absolute, out var parsedUri);
            return parsedUri;
        }
    }
}
