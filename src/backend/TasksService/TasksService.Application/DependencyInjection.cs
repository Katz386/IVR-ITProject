using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksService.Application.Interfaces;
using TasksService.Application.Service;

namespace TasksService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ITasksService, TasksService.Application.Service.TasksService>();

            return services;
        }
    }
}
