using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Interfaces
{
    public interface IServiceBus
    {
        public Task PublishAsync<T>(T message, CancellationToken ct = default) where T : class;
    }
}
