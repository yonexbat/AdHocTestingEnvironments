using AdHocTestingEnvironments.Model.EnvironmentConfig;
using AdHocTestingEnvironments.Routing;
using AdHocTestingEnvironments.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yarp.ReverseProxy.Forwarder;

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

            services.Configure<EnvironmentConfigOptions>(Configuration.GetSection("EnvironmentConfig"));

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddHttpContextAccessor();

            services.AddHttpForwarder();

            services.AddSingleton<RequestRouter>();
            services.AddSingleton<IRoutingService, RoutingService>();
            services.AddScoped<IEnvironmentService, EnvironmentService>();
            services.AddScoped<IKubernetesClient, KubernetesClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpForwarder forwarder, RequestRouter requestRouter)
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
        }
    }
}
