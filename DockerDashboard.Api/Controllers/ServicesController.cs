using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using DockerDashboard.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DockerDashboard.Api.Controllers;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
[Route("api/services")]
public class ServicesController : ControllerBase
{
    private readonly ILogger<ServicesController> _logger;
    private readonly Uri _swarmUrl;

    public ServicesController(ILogger<ServicesController> logger, IOptions<AppConfiguration> options)
    {
        _logger = logger;
        _swarmUrl = options.Value.SwarmUrl;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SwarmServiceDto>>> GetServicesAsync()
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
            var port = service.Endpoint.Ports.FirstOrDefault();

            result.Add(new SwarmServiceDto
            {
                Name = service.Spec.Name,
                Ports = service.Endpoint.Ports,
                SwaggerUrl = CreateSwaggerUri(port)
            });
        }

        return Ok(result);
    }

    private Uri CreateSwaggerUri(PortConfig port)
    {
        if (port == null)
        {
            return null;
        }

        var uriBuilder = new UriBuilder(_swarmUrl.AbsoluteUri)
        {
            Port = (int)port.PublishedPort,
            Path = "swagger"
        };
        return uriBuilder.Uri;
    }
}