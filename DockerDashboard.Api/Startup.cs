using System.Net.Http.Headers;
using DockerDashboard.Api.Configuration;
using DockerDashboard.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DockerDashboard.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });
        });

        services.Configure<SwarmConfiguration>(Configuration.GetSection(SwarmConfiguration.SectionName));

        var registryConfig = Configuration.GetSection(RegistryConfiguration.SectionName).Get<RegistryConfiguration>();

        services.AddHttpClient<IDockerRegistryProxy, DockerRegistryProxy>(client =>
        {
            client.BaseAddress = registryConfig.Url;
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", registryConfig.AuthToken);
        });

        services.AddControllers();
        services.AddMvc(option => option.EnableEndpointRouting = false);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors("CorsPolicy");

        app.UseMvc();
        app.UseRouting();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
