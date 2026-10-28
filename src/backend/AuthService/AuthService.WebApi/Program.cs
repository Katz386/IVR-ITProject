using AuthService.Application;
using AuthService.Domain;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Persistence;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables();

        builder.WebHost.UseUrls(builder.Configuration["ASPNETCORE_URLS"]);

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddControllers();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        await AdminSeeder.SeedAdminAsync(app.Services, builder.Configuration);

        app.UseForwardedHeaders();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
