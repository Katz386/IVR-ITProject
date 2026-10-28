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
    public class AccountCreatedConsumer(IUserService profileService) : IConsumer<AccountCreated>
    {
        public async Task Consume(ConsumeContext<AccountCreated> context)
        {
            await profileService.CreateProfile(context.Message.AccountId);
        }
    }
}
