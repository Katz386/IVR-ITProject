using FileService.Application;
using FileService.Infrastructure;
using FileService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables();

        builder.WebHost.UseUrls(builder.Configuration["ASPNETCORE_URLS"]);

        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddApplication();

        builder.Services
            .AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors.Select(error => new
                        {
                            Field = x.Key,
                            Message = error.ErrorMessage
                        }));

                    return new BadRequestObjectResult(new
                    {
                        Status = 400,
                        Message = "Request validation failed",
                        Details = errors
                    });
                };
            });

        var secret = builder.Configuration["Jwt:Secret"];

        if (string.IsNullOrEmpty(secret))
        {
            Console.WriteLine("Missing JWT secret in env!!!");
        }

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                RoleClaimType = ClaimTypes.Role,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        WebApplication app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        app.UseForwardedHeaders();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
