using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Throttler
{
    public class RateLimiterOptions
    {
        public TimeSpan Rate { get; private set; }

        public long Limit { get; private set; } 


        public RateLimiterOptions(long limit, TimeSpan rate)
        {
            this.Limit = limit;
            this.Rate = rate;
        }
    }
}
