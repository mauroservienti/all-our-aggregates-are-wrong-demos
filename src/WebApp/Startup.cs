using ITOps.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.Shared.Hosting;
using ServiceComposer.AspNetCore;
using ServiceComposer.AspNetCore.Mvc;

namespace WebApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddNServiceBus("WebApp", endpointConfiguration =>
            {
                endpointConfiguration.ApplyCommonConfiguration(asSendOnly: true);
            });
            services.AddControllersWithViews();
            services.AddViewModelComposition(options =>
            {
                options.AddMvcSupport();
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseBrowserLink();

            app.UseStaticFiles();
            app.UseMiddleware<ShoppingCartMiddleware>();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
