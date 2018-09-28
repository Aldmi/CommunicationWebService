using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Extensions
{
    public static class MyTaskExtensions
    {
        public static Task<T> WithTimeout<T>(this Task<T> task, int duration, CancellationToken ct)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    bool res = task.Wait(duration, ct);
                    return res ? task.Result : default(T);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.InnerException != null)
                        {
                            throw ex.InnerException.InnerException;
                        }
                        throw ex.InnerException;
                    }
                   throw;
                }
            }, ct);
        }

        public static async Task WaitInfinityTask(this Task task, CancellationToken ct)
        {
            try
            {
                await Task.Delay(-1, ct);
            }
            catch (OperationCanceledException)
            { }
        }
  
    }
}