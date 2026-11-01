using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Application.Interfaces;
using PlatformService.Infrastructure.Consumers;
using PlatformService.Infrastructure.Persistence;
using PlatformService.Infrastructure.Repository;
using SharedContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration["PlatformService_DB_ConnectionString"]));
            services.AddScoped<IPlatformRepository, PlatformRepository>();
            services.AddMassTransit(x =>
            {
                x.AddConsumer<ArticleCreatedConsumer>().Endpoint(e => e.Name = "article-created");
                x.AddConsumer<ArticleDeletedConsumer>().Endpoint(e => e.Name = "article-deleted");
                x.AddConsumer<TaskCreatedConsumer>().Endpoint(e => e.Name = "task-created");
                x.AddConsumer<TaskDeletedConsumer>().Endpoint(e => e.Name = "task-deleted");
                x.AddConsumer<AccountCreatedStatisticsConsumer>().Endpoint(e => e.Name = "account-created-statistics");

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RABBITMQ_HOST"], configuration["RABBITMQ_VHOST"], h =>
                    {
                        h.Username(configuration["RABBITMQ_USER"]);
                        h.Password(configuration["RABBITMQ_PASS"]);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
