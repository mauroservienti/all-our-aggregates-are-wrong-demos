using ITOps.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ServiceComposer.AspNetCore;

namespace WebApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
            services.AddControllersWithViews();
            services.AddViewModelComposition();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseBrowserLink();

            app.UseStaticFiles();
            app.UseMiddleware<ShoppingCartMiddleware>();
            app.UseMiddleware<TransactionalSessionMiddleware>();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                    .WithCompositionOverControllers();
                endpoints.MapCompositionHandlers();
            });
        }
    }
}
