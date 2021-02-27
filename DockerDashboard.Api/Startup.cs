using System;
using System.Net.Http.Headers;
using System.Text;
using DockerDashboard.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DockerDashboard.Api
{
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
            services.AddSpaStaticFiles(options => { options.RootPath = "wwwroot"; });
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

            services.AddHttpClient<IDockerRegistryProxy, DockerRegistryProxy>(client =>
            {
                var dockerPwd = Configuration["dockerregistrysecrets"];
                var dockerUser = Configuration["Settings:DockerRegistryUser"];
                var authToken = Encoding.ASCII.GetBytes($"{dockerUser}:{dockerPwd}");

                client.BaseAddress = new Uri(Configuration["Settings:DockerRegistryUrl"]);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            });

            services.AddControllers();
            services.AddMvc(option => option.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("VueCorsPolicy");

            app.UseMvc();
            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseSpaStaticFiles();
            app.UseSpa(builder =>
            {
                if (env.IsDevelopment())
                {
                    builder.UseProxyToSpaDevelopmentServer("http://localhost:8080");
                }
            });
        }
    }
}
