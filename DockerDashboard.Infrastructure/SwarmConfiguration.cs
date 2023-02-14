using System;

namespace DockerDashboard.Infrastructure;

public record SwarmConfiguration
{
    public const string SectionName = "Swarm";

    public Uri Url { get; init; }
}
