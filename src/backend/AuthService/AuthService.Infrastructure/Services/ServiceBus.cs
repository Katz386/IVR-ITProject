using AuthService.Application.Interfaces;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Services
{
    public class ServiceBus : IServiceBus
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public ServiceBus(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task PublishAsync<T>(T message, CancellationToken ct = default) where T : class
        {
            return _publishEndpoint.Publish(message, ct);
        }
    }
}
