using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Throttler
{
    internal interface IRequest
    {
        Func<Task> Action { get; }

        Guid Id { get; }
    }
}
