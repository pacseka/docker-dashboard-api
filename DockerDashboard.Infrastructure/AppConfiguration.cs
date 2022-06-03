using System;

namespace DockerDashboard.Infrastructure;

public record AppConfiguration
{
    public static string SectionName = "Settings";

    public Uri DockerRegistryUrl { get; init; }

    public string DockerRegistryUser { get; init; }

    public Uri SwarmUrl { get; init; }
}