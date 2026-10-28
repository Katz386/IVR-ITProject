using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using UserService.Infrastructure.Consumers;
using UserService.Infrastructure.Persistence;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Interfaces;
using UserService.Infrastructure.Repository;

namespace UserService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration["UserService_DB_ConnectionString"]));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddMassTransit(x =>
            {
                x.AddConsumer<AccountCreatedConsumer>().Endpoint(e => e.Name = "profile-account-created");
                x.AddConsumer<TaskSolvedConsumer>().Endpoint(e => e.Name = "profile-task-solved");

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
