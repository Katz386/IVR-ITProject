using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedContracts
{
    public record AccountCreated (Guid EventId, Guid AccountId, DateTime Time);
}
