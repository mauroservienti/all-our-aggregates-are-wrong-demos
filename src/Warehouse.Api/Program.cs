using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Warehouse.Api;

public class Program
{
    public static void Main(string[] args) => Build(args).Run();

    public static WebApplication Build(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddCors(options =>
            options.AddPolicy("AllowAllOrigins", b => b.AllowAnyOrigin()));
        builder.Services.AddControllers();

        var app = builder.Build();
        app.UseCors("AllowAllOrigins");
        app.MapControllers();

        return app;
    }
}
