﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Throttler.tests
{
    [TestClass]
    public class RequestLimiterTests
    {
        [TestMethod]
        public async Task RequestLimiterTest()
        {
            RateLimiter rateLimiter = new(new RateLimiterOptions(10, TimeSpan.FromSeconds(1)));

            Stopwatch watch = Stopwatch.StartNew();

            List<Task> tasks = new(100);

            int num = 0;

            for (int i=0;i<100; i++)
            {
                tasks.Add(rateLimiter.ExecuteAsync(funcBody));
            }

            await Task.WhenAll(tasks);

            watch.Stop();

            Assert.IsTrue(watch.Elapsed.TotalSeconds >= 10);

            async Task funcBody()
            {
                Logger.LogMessage($"Executing {Interlocked.Increment(ref num)} Execution Time: {DateTime.Now}");

                await Task.Delay(1000);
            }
        }
    }
}
