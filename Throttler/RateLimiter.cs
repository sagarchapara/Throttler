using System.Collections.Concurrent;

namespace Throttler
{
    public class RateLimiter : IRateLimiter
    {
        private readonly ConcurrentQueue<IRequest> userRequestQueue;

        private readonly ConcurrentDictionary<Guid, TaskCompletionSource> userRequestTasks;

        private readonly RateLimiterOptions options;

        private readonly System.Timers.Timer timer;

        private long currentTokens;

        public RateLimiter(RateLimiterOptions options)
        {
            this.options = options;
            this.userRequestQueue = new ConcurrentQueue<IRequest>();
            this.userRequestTasks = new ConcurrentDictionary<Guid, TaskCompletionSource>();

            this.currentTokens = this.options.Limit;

            this.timer = new System.Timers.Timer
            {
                AutoReset = true,
                Enabled = true,
                Interval = this.options.Rate.TotalMilliseconds
            };

            this.timer.Elapsed += ResetTokens;
        }

        private void ResetTokens(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Interlocked.Exchange(ref this.currentTokens, this.options.Limit);

            this.StartWorker();
        }

        public Task ExecuteAsync(Func<Task> action)
        {
            Guid id = Guid.NewGuid();

            Request request = new Request(id, action);

            this.userRequestQueue.Enqueue(request);

            TaskCompletionSource taskCompletionSource = new TaskCompletionSource();

            userRequestTasks.TryAdd(id, taskCompletionSource);

            return taskCompletionSource.Task;
        }

        private int isRunning = 0;

        private void StartWorker()
        {
            try
            {
                if (Interlocked.Exchange(ref this.isRunning, 1) == 0)
                {
                    while(Interlocked.Decrement(ref this.currentTokens) >= 0)
                    {
                        if(this.userRequestQueue.TryDequeue(out IRequest request))
                        {
                            if(this.userRequestTasks.TryGetValue(request.Id, out TaskCompletionSource tcs))
                            {
                                _ = Task.Run(async () =>
                                {
                                    try
                                    {
                                        //Execute
                                        await request.Action();

                                        tcs.SetResult();
                                    }
                                    catch (Exception ex)
                                    {
                                        tcs.SetException(ex);
                                    }
                                });
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            finally
            {
                Interlocked.Exchange (ref this.isRunning, 0);
            }
        }
    }
}