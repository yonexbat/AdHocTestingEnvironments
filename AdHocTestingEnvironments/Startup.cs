using AdHocTestingEnvironments.Services.Implementations;
using AdHocTestingEnvironments.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yarp.ReverseProxy.Forwarder;
using Microsoft.EntityFrameworkCore;
using AdHocTestingEnvironments.Data;
using System;

namespace AdHocTestingEnvironments
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

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddHttpContextAccessor();

            services.AddHttpForwarder();

            services.AddSingleton<IRequestRouterService, RequestRouterService>();
            services.AddScoped<IEndpointResolverService, EndpointResolverService>();
            services.AddSingleton<IKubernetesFactory, KubernetesFactory>();
            services.AddSingleton<ICurrentTimeService, CurrentTimeService>();
            services.AddSingleton<IKubernetesObjectBuilder, KubernetesObjectBuilder>();
            services.AddSingleton<IGitClientService, GitClientService>();
            services.AddScoped<IEnvironmentInstanceService, EnvironmentInstanceService>();
            services.AddScoped<IEnvironmentService, EnvironmentService>();
            services.AddScoped<IKubernetesClientService, KubernetesClientService>();
            services.AddScoped<IEnvironmentKillerService, EnvironmentKillerService>();

            services.AddHostedService<TimerBackgroundService>();
            AddDbContext(services);
        }

        private void AddDbContext(IServiceCollection services)
        {
            string databaseTech = Configuration.GetValue<string>("DatabaseTech");

            switch (databaseTech)
            {
                case "InMemory":
                    AddDbContextInMemory(services);
                    break;
                case "NpgSql":
                    AddDbContextNpgSql(services);
                    break;
                default:
                    throw new ArgumentException($"Configuration value DatabaseTech is invalid: {databaseTech}. Must be InMemory or NpgSql");
            }
        }

        private void AddDbContextInMemory(IServiceCollection services)
        {            
            services.AddDbContext<AdHocTestingEnvironmentsContext>(options =>
                    options.UseInMemoryDatabase($"AdHocTestingEnvironments"));
        }

        private void AddDbContextNpgSql(IServiceCollection services)
        {
            
            string connectionString = Configuration.GetConnectionString("SampleWebAppContext");

            services.AddDbContext<AdHocTestingEnvironmentsContext>(options =>
                   options.UseNpgsql(connectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env, 
            IHttpForwarder forwarder, 
            IRequestRouterService requestRouter,
            IGitClientService gitClientService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();           

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                endpoints.MapControllerRoute(
                 name: "default",
                 pattern: "{controller}/{action=Index}/{id?}");

                // When using IHttpForwarder for direct forwarding you are responsible for routing, destination discovery, load balancing, affinity, etc..
                // For an alternate example that includes those features see BasicYarpSample.
                endpoints.Map("/endpoint/{**remainder}", async httpContext =>
                {
                    await requestRouter.RouteRequest(httpContext, forwarder);
                });
            });

            if(Configuration.GetValue<bool>("UseGitClient"))
            {
                gitClientService.CheckOut().Wait();
            }
            if (Configuration.GetValue<bool>("InitDb"))
            {
                app.InitAdHocTestingEnvironmentDb();
            }               
        }
    }
}
