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
    public class ArticleDeletedConsumer(IPlatformService platformService) : IConsumer<ArticleDeleted>
    {
        public async Task Consume(ConsumeContext<ArticleDeleted> context)
        {
            await platformService.DecreaseArticlesCount();
        }
    }
}
