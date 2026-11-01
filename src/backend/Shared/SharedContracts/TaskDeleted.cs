using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedContracts
{
    public record TaskDeleted(Guid EventId, int taskId, string Difficulty, DateTime Time);
}
