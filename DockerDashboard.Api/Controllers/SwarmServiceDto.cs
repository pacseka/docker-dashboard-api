using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Docker.DotNet.Models;

namespace DockerDashboard.Api.Controllers;

public class SwarmServiceDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public Uri SwaggerUrl { get; set; }

    public ICollection<PortConfig> Ports { get; set; }
}
