using Microsoft.Extensions.DependencyInjection;
using PlatformService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IPlatformService, PlatformService.Application.Services.PlatformService>();
            return services;
        }
    }
}
