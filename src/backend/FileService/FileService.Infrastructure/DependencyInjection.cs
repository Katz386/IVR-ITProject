using FileService.Application.Interfaces;
using FileService.Infrastructure.Persistence;
using FileService.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration["FileService_DB_ConnectionString"]));
            services.AddScoped<IFileServiceRepository, BasicFileStorageRepository>();

            return services;
        }
    }
}
