using System;
using System.Text;

namespace DockerDashboard.Api.Configuration;

public record RegistryConfiguration
{
    public const string SectionName = "Registry";

    public Uri Url { get; init; }

    public string User { get; init; }

    public string Password { get; init; }

    public string AuthToken => Convert.ToBase64String(Encoding.ASCII.GetBytes($"{User}:{Password}"));
}
