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
    public class ArticleCreatedConsumer(IPlatformService platformService) : IConsumer<ArticleCreated>
    {
        public async Task Consume(ConsumeContext<ArticleCreated> context)
        {
            await platformService.IncreaseArticlesCount();
        }
    }
}
