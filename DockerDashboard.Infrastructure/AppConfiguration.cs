using System;

namespace DockerDashboard.Infrastructure
{
    public class AppConfiguration
    {
        public Uri DockerRegistryUrl { get; set; }

        public string DockerRegistryUser { get; set; }
    }
}
