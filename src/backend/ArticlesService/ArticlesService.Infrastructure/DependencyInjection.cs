using ArticlesService.Application.Interfaces;
using ArticlesService.Infrastructure.Persistence;
using ArticlesService.Infrastructure.Repository;
using ArticlesService.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration["ArticlesService_DB_ConnectionString"]));

            services.AddScoped<IArticlesRepository, ArticlesRepository>();

            services.AddScoped<IServiceBus, ServiceBus>();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RABBITMQ_HOST"], configuration["RABBITMQ_VHOST"], h =>
                    {
                        h.Username(configuration["RABBITMQ_USER"]);
                        h.Password(configuration["RABBITMQ_PASS"]);
                    });
                });
            });

            return services;
        }
    }
}
