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
    public class TaskDeletedConsumer(IPlatformService platformService) : IConsumer<TaskDeleted>
    {
        public async Task Consume(ConsumeContext<TaskDeleted> context)
        {
            await platformService.DecreaseTasksCount(context.Message.Difficulty);
        }
    }
}
