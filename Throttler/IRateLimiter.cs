using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Throttler
{
    internal interface IRateLimiter
    {
        Task ExecuteAsync(Func<Task> action);
    }
}
