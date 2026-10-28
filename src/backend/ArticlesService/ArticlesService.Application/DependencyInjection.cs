using ArticlesService.Application.Interfaces;
using ArticlesService.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IArticlesService, ArticlesService.Application.Services.ArticlesService>();

            return services;
        }
    }
}
