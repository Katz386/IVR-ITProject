using MassTransit;
using SharedContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure.Consumers
{
    public class TaskSolvedConsumer(IUserService profileService) : IConsumer<TaskSolved>
    {
        public async Task Consume(ConsumeContext<TaskSolved> context)
        {
            await profileService.SolveTask(context.Message.AccountId, context.Message.TaskId);
        }
    }
}
