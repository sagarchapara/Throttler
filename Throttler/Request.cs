using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Throttler
{
    internal class Request : IRequest
    {
        public Func<Task> Action { get; set; }

        public Guid Id { get; set; }

        public Request(Guid id, Func<Task> action)
        {
            this.Action = action;
            Id = id;
        }
    }
}
