using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedContracts
{
    public record TaskSolved (Guid EventId, Guid AccountId, int TaskId, DateTime Time);
}
