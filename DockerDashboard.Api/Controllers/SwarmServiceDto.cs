using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Docker.DotNet.Models;

namespace DockerDashboard.Api.Controllers;

public class SwarmServiceDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public Uri SwaggerUrl { get; set; }

    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "DTO")]
    public ICollection<PortConfig> Ports { get; set; }
}