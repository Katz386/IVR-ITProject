using MassTransit;
using PlatformService.Application.Interfaces;
using SharedContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformService.Infrastructure.Consumers
{
    public class TaskCreatedConsumer(IPlatformService platformService) : IConsumer<TaskCreated>
    {
        public async Task Consume(ConsumeContext<TaskCreated> context)
        {
            await platformService.IncreaseTasksCount(context.Message.Difficulty);
        }
    }
}
