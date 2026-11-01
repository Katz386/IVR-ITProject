using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedContracts
{
    public record TaskCreated (Guid EventId, string Difficulty, DateTime Time);
}
