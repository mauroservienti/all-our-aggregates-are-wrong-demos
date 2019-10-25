using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Shared.Hosting;
using NServiceBus;

namespace Sales.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin();
                });
            });

            services.AddNServiceBus("Sales.Api", endpointConfiguration =>
            {
                endpointConfiguration.ApplyCommonConfiguration(asSendOnly: true);
            });
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseCors("AllowAllOrigins");
            app.UseMvc();
        }
    }
}
