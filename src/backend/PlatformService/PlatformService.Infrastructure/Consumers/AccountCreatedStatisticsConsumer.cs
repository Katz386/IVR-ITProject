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
    public class AccountCreatedStatisticsConsumer(IPlatformService platformService) : IConsumer<AccountCreatedStatistics>
    {
        public async Task Consume(ConsumeContext<AccountCreatedStatistics> context)
        {
            await platformService.IncreaseUsersCount();
        }
    }
}
