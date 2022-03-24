using ITOps.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ServiceComposer.AspNetCore;

namespace CompositionGateway
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
            services.AddViewModelComposition(options =>
            {
                options.EnableWriteSupport();
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<ShoppingCartMiddleware>();

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapCompositionHandlers());
        }
    }
}
